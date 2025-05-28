/* 2021/5/9 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TerminalMonitor.Clipboard;
using TerminalMonitor.Execution;
using TerminalMonitor.Matchers;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using TerminalMonitor.Models.Settings;
using TerminalMonitor.Terminal;
using TerminalMonitor.Windows.Converters;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalView.xaml
    /// </summary>
    public partial class TerminalView : UserControl
    {
        private ITerminalSupervisor? terminalLineSupervisor;

        private const string idColumnName = "Id";

        private const string plaintextColumnName = "PlainText";

        private const string invalidNumber = "?";

        private const string beforeLineNumber = "-";

        private const string afterLineNumber = "+";

        private readonly DataTable terminalDataTable = new();

        private List<FieldDisplayDetail> visibleFields = [];

        private readonly TerminalViewColumnSettingHelper columnSettingHelper = new();

        private GroupCondition filterCondition = GroupCondition.Empty;

        private GroupCondition findCondition = GroupCondition.Empty;

        private readonly TerminalViewDataContextVO dataContextVO;

        private readonly Dictionary<string, bool> lineFilterDict = [];

        private readonly List<TerminalLine> shownLines = [];

        private readonly List<(TerminalLine terminalLine, int shownIndex)> foundLines = [];

        private string? clickedColumnFieldKey;

        private string? clickedRowTerminalLineId;

        public TerminalView()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                FoundSelectedNumber = invalidNumber,
                FoundCount = 0,
            };

            DataContext = dataContextVO;
            listTerminal.DataContext = dataContextVO;
            listTerminal.ContextMenu.DataContext = dataContextVO;
            ApplyVisibleField();
        }

        private void ButtonApplyFields_Click(object sender, RoutedEventArgs e)
        {
            ApplyVisibleField();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterTerminal();
        }

        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            FindInTerminal();
        }

        private void ButtonFindPrevious_Click(object sender, RoutedEventArgs e)
        {
            FindPrevious();
        }

        private void ButtonFindNext_Click(object sender, RoutedEventArgs e)
        {
            FindNext();
        }

        private void ButtonFindFirst_Click(object sender, RoutedEventArgs e)
        {
            FindFirst();
        }

        private void ButtonFindLast_Click(object sender, RoutedEventArgs e)
        {
            FindLast();
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            var clipboardContainsText = CheckSystemClipboard();
            menuItemPastePlaintextFilter.IsEnabled = clipboardContainsText;
            menuItemPastePlaintextFind.IsEnabled = clipboardContainsText;
        }

        private void MenuItemShowDetail_Click(object sender, RoutedEventArgs e)
        {
            ShowDetailWindow(clickedRowTerminalLineId);
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTerminal();
        }

        private void MenuItemAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            dataContextVO.AutoScroll = !dataContextVO.AutoScroll;
        }

        private void MenuItemAddFilterCondition_Click(object sender, RoutedEventArgs e)
        {
            AddClickedCellToConditionListView(filterConditionListView);
        }

        private void MenuItemAddFindCondtion_Click(object sender, RoutedEventArgs e)
        {
            AddClickedCellToConditionListView(findConditionListView);
        }

        private void MenuItemCopyPlaintext_Click(object sender, RoutedEventArgs e)
        {
            CopyClickedDataCellToSystemClipboard();
        }

        private void MenuItemPastePlaintextFilter_Click(object sender, RoutedEventArgs e)
        {
            PasteSystemClipboardToConditionListView(filterConditionListView);
        }

        private void MenuItemPastePlaintextFind_Click(object sender, RoutedEventArgs e)
        {
            PasteSystemClipboardToConditionListView(findConditionListView);
        }

        private void ListTerminal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFoundSelectedNumber();
        }

        private void ListTerminalItemContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender is ListViewItem item) && (item.Content is DataRowView rowView))
            {
                clickedRowTerminalLineId = (string)rowView[idColumnName];
            }
        }

        private void ListTerminalItemContainer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender is ListViewItem item) && (item.Content is DataRowView rowView))
            {
                clickedRowTerminalLineId = (string)rowView[idColumnName];

                ShowDetailWindow(clickedRowTerminalLineId);
            }
        }

        private void ClearTerminal()
        {
            if (listTerminal.SelectedValue is DataRowView item)
            {
                var id = (string)item[idColumnName];
                terminalLineSupervisor?.RemoveTerminalLinesUntil(id);
            }
        }

        private void ShowDetailWindow(string? terminalLineId)
        {
            if (terminalLineId != null)
            {
                var terminalLine = terminalLineSupervisor?.TerminalLines[terminalLineId];

                if (terminalLine != null)
                {
                    TerminalLineDetailWindow window = new()
                    {
                        TerminalLine = terminalLine,
                        ConditionListClipboard = findConditionListView.ConditionListClipboard,
                        ConditionTreeClipboard = findConditionListView.ConditionTreeClipboard,
                    };
                    window.Show();
                    Task.Delay(TimeSpan.FromMilliseconds(100))
                        .ContinueWith(_ =>
                        {
                            Application.Current.Dispatcher.Invoke(() => window.Focus());
                        });
                }
            }
        }

        private void AddClickedCellToConditionListView(ConditionListView conditionListView)
        {
            var condition = ConvertClickedDataCellToCondition();
            if (condition != null)
            {
                conditionListView.AddCondition(condition);
            }
        }

        private string? GetClickedDataCell()
        {
            if (clickedRowTerminalLineId == null || clickedColumnFieldKey == null)
            {
                return null;
            }

            var terminalLine = terminalLineSupervisor?.TerminalLines[clickedRowTerminalLineId];
            if (terminalLine == null)
            {
                return null;
            }

            var terminalLineFieldDict = terminalLine.LineFieldDict;
            if (!terminalLineFieldDict.TryGetValue(clickedColumnFieldKey, out TerminalLineField? lineFieldValue))
            {
                return null;
            }

            return lineFieldValue.Text;
        }

        private FieldCondition? ConvertClickedDataCellToCondition()
        {
            if (clickedColumnFieldKey == null)
            {
                return null;
            }

            var clickedDataCell = GetClickedDataCell();
            if (clickedDataCell == null)
            {
                return null;
            }

            return new FieldCondition(
                clickedColumnFieldKey,
                TextMatchOperator.Equals,
                clickedDataCell);
        }

        private void CopyClickedDataCellToSystemClipboard()
        {
            var clickedDataCell = GetClickedDataCell();
            if (clickedDataCell == null)
            {
                return;
            }

            System.Windows.Clipboard.SetText(clickedDataCell, TextDataFormat.UnicodeText);
        }

        private static bool CheckSystemClipboard()
        {
            return System.Windows.Clipboard.ContainsText(TextDataFormat.UnicodeText);
        }

        private void PasteSystemClipboardToConditionListView(ConditionListView conditionListView)
        {
            if (clickedColumnFieldKey == null)
            {
                return;
            }

            if (!CheckSystemClipboard())
            {
                return;
            }

            var condition = new FieldCondition(
                clickedColumnFieldKey,
                TextMatchOperator.Contains,
                System.Windows.Clipboard.GetText(TextDataFormat.UnicodeText));

            conditionListView.AddCondition(condition);
        }

        public void AddNewTerminalLines(IEnumerable<TerminalLine> terminalLineDtos)
        {
            var isAnyAdded = false;
            foreach (var terminalLineDto in terminalLineDtos)
            {
                var matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterCondition);
                lineFilterDict.Add(terminalLineDto.Id, matched);

                if (matched)
                {
                    shownLines.Add(terminalLineDto);
                    AddTerminalLine(terminalLineDto);
                    isAnyAdded = true;
                }
            }

            if (isAnyAdded && dataContextVO.AutoScroll)
            {
                var itemCount = listTerminal.Items.Count;
                var lastItem = listTerminal.Items[itemCount - 1];
                listTerminal.ScrollIntoView(lastItem);
            }
        }

        private void FilterTerminal()
        {
            filterCondition = (GroupCondition)filterConditionListView.Condition.Clone();

            if (terminalLineSupervisor == null)
            {
                return;
            }

            terminalDataTable.Rows.Clear();

            lineFilterDict.Clear();
            shownLines.Clear();
            TerminalLineMatcher matcher = new(filterCondition);
            foreach (var terminalLineDto in terminalLineSupervisor.TerminalLines)
            {
                var matched = matcher.IsMatch(terminalLineDto);
                lineFilterDict.Add(terminalLineDto.Id, matched);

                if (matched)
                {
                    shownLines.Add(terminalLineDto);
                }
            }

            AddMatchedTerminalLines();

            FindInTerminal();
        }

        private void FindInTerminal()
        {
            findCondition = (GroupCondition)findConditionListView.Condition.Clone();
            foundLines.Clear();

            TerminalLineMatcher matcher = new(findCondition);
            for (var i = 0; i < shownLines.Count; i++)
            {
                var terminalLineDto = shownLines[i];
                var found = matcher.IsMatch(terminalLineDto);
                if (found)
                {
                    foundLines.Add((terminalLine: terminalLineDto, shownIndex: i));
                }
            }

            dataContextVO.FoundCount = foundLines.Count;

            UpdateFoundSelectedNumber();
        }

        private void UpdateFoundSelectedNumber()
        {
            if (foundLines.Count == 0)
            {
                dataContextVO.FoundSelectedNumber = invalidNumber;
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            if (selectedIndex == -1)
            {
                dataContextVO.FoundSelectedNumber = invalidNumber;
            }
            else if (selectedIndex < foundLines.First().shownIndex)
            {
                dataContextVO.FoundSelectedNumber = beforeLineNumber;
            }
            else if (selectedIndex > foundLines.Last().shownIndex)
            {
                dataContextVO.FoundSelectedNumber = afterLineNumber;
            }
            else
            {

                for (int i = 0; i < foundLines.Count; i++)
                {
                    var (_, shownIndex) = foundLines[i];

                    if (shownIndex == selectedIndex)
                    {
                        dataContextVO.FoundSelectedNumber = (i + 1).ToString();

                        break;
                    }
                    else if (shownIndex > selectedIndex)
                    {
                        dataContextVO.FoundSelectedNumber = $"{i}{afterLineNumber}";
                        break;
                    }
                }
            }
        }

        private void FindPrevious()
        {
            if (foundLines.Count == 0)
            {
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            if (selectedIndex == -1)
            {
                FindLast();
                return;
            }

            for (int i = foundLines.Count - 1; i >= 0; i--)
            {
                var (_, shownIndex) = foundLines[i];
                if (shownIndex < selectedIndex)
                {
                    listTerminal.SelectedIndex = shownIndex;
                    listTerminal.ScrollIntoView(listTerminal.SelectedItem);

                    dataContextVO.FoundSelectedNumber = (i + 1).ToString();
                    return;
                }
            }

            FindLast();
        }

        private void FindNext()
        {
            if (foundLines.Count == 0)
            {
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            if (selectedIndex == -1)
            {
                FindFirst();
                return;
            }

            for (int i = 0; i < foundLines.Count; i++)
            {
                var (_, shownIndex) = foundLines[i];
                if (shownIndex > selectedIndex)
                {
                    listTerminal.SelectedIndex = shownIndex;
                    listTerminal.ScrollIntoView(listTerminal.SelectedItem);

                    dataContextVO.FoundSelectedNumber = (i + 1).ToString();
                    return;
                }
            }

            FindFirst();
        }

        private void FindFirst()
        {
            if (foundLines.Count == 0)
            {
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            var (_, shownIndex) = foundLines.First();

            if (selectedIndex != shownIndex)
            {
                listTerminal.SelectedIndex = shownIndex;
                listTerminal.ScrollIntoView(listTerminal.SelectedItem);
            }

            dataContextVO.FoundSelectedNumber = 1.ToString();
        }

        private void FindLast()
        {
            if (foundLines.Count == 0)
            {
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            var (_, shownIndex) = foundLines.Last();

            if (selectedIndex != shownIndex)
            {
                listTerminal.SelectedIndex = shownIndex;
                listTerminal.ScrollIntoView(listTerminal.SelectedItem);
            }

            dataContextVO.FoundSelectedNumber = foundLines.Count.ToString();
        }

        private void ApplyVisibleField()
        {
            // Save current column settings before reload grid.
            var gridViewColumnSettings = columnSettingHelper.GetGridViewColumnSettings();

            visibleFields = columnSettingHelper.Init(fieldListView.Fields);

            GridView gridView = new();

            var isAnyColumnHeaderCustomized =
                    visibleFields.Any(visibleField => visibleField.CustomizeHeader && !visibleField.Hidden);

            if (isAnyColumnHeaderCustomized)
            {
                var gridViewHeaderStyle = new Style(typeof(GridViewColumnHeader));

                /* Default style
                gridViewHeaderStyle.Setters.Add(
                    new Setter(GridViewColumnHeader.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
                gridViewHeaderStyle.Setters.Add(
                    new Setter(GridViewColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
                */

                gridViewHeaderStyle.Setters.Add(
                    new Setter(GridViewColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
                gridView.ColumnHeaderContainerStyle = gridViewHeaderStyle;
            }

            terminalDataTable.Columns.Clear();
            terminalDataTable.Rows.Clear();

            DataColumn idColumn = new(idColumnName)
            {
                DataType = typeof(string)
            };
            terminalDataTable.Columns.Add(idColumn);

            if (visibleFields.Count != 0)
            {
                /*
                 * Add selected visible fields.
                 */
                foreach (var visibleField in visibleFields)
                {
                    if (visibleField.Hidden)
                    {
                        continue;
                    }

                    DataColumn column = new(visibleField.Id)
                    {
                        DataType = typeof(string)
                    };
                    terminalDataTable.Columns.Add(column);

                    GridViewColumn viewColumn = new();

                    if (visibleField.CustomizeHeader)
                    {
                        var columnTemplate = TerminalViewHelper.BuildColumnHeaderTemplate(visibleField);
                        viewColumn.HeaderTemplate = columnTemplate;
                    }
                    else if (isAnyColumnHeaderCustomized)
                    {
                        var columnTemplate = TerminalViewHelper.BuildDefaultColumnHeaderTemplate(visibleField);
                        viewColumn.HeaderTemplate = columnTemplate;
                    }
                    else
                    {
                        string headerText = visibleField.HeaderName ?? visibleField.FieldKey;
                        // Escape the underscore that is access key indicator.
                        headerText = headerText.Replace("_", "__");
                        viewColumn.Header = headerText;
                    }

                    if (visibleField.CustomizeStyle)
                    {
                        MouseButtonEventHandler mouseDownHandler = (sender, e) =>
                        {
                            clickedColumnFieldKey = visibleField.FieldKey;
                        };

                        DataTemplate dataTemplate = TerminalViewHelper.BuildFieldDataTemplate(
                            visibleField, terminalDataTable,
                            [
                                new(MouseDownEvent, mouseDownHandler)
                            ]);

                        viewColumn.CellTemplate = dataTemplate;
                    }
                    else
                    {
                        viewColumn.DisplayMemberBinding = new Binding(visibleField.Id);
                    }

                    gridView.Columns.Add(viewColumn);
                    columnSettingHelper.AddColumn(visibleField.Id, viewColumn);
                }
            }
            else
            {
                /*
                 * Add default column when no visible field selected.
                 */
                DataColumn defaultColumn = new(plaintextColumnName)
                {
                    DataType = typeof(string)
                };
                terminalDataTable.Columns.Add(defaultColumn);

                gridView.Columns.Add(new GridViewColumn()
                {
                    Header = plaintextColumnName,
                    DisplayMemberBinding = new Binding(plaintextColumnName),
                });
            }

            // Recover column settings. 
            columnSettingHelper.SetGridViewColumnSettings(gridViewColumnSettings);

            AddMatchedTerminalLines();

            listTerminal.View = gridView;
            Binding binding = new();
            listTerminal.DataContext = terminalDataTable;
            listTerminal.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private void AddTerminalLine(TerminalLine terminalLineDto)
        {
            DataRow row = terminalDataTable.NewRow();

            row[idColumnName] = terminalLineDto.Id;

            if (visibleFields.Count != 0)
            {
                foreach (var visibleField in visibleFields)
                {
                    if (visibleField.Hidden)
                    {
                        continue;
                    }

                    var fieldValue = terminalLineDto.LineFieldDict.TryGetValue(visibleField.FieldKey, out TerminalLineField? value) ? value.Text : "";

                    row[visibleField.Id] = fieldValue;

                    if (visibleField.CustomizeStyle)
                    {
                        var matchedTextStyleCondition = visibleField.Conditions?.FirstOrDefault(
                            textStyleCondition => TerminalLineMatcher.IsMatch(terminalLineDto, textStyleCondition.Condition));

                        TerminalViewHelper.BuildDataRowStyleCells(row, visibleField, matchedTextStyleCondition?.Style);
                    }
                }
            }
            else
            {
                row[plaintextColumnName] = terminalLineDto.PlainText;
            }

            terminalDataTable.Rows.Add(row);
        }

        private void AddMatchedTerminalLines()
        {
            if (terminalLineSupervisor == null)
            {
                return;
            }

            foreach (var terminalLineDto in terminalLineSupervisor.TerminalLines)
            {
                bool matched;
                if (lineFilterDict.TryGetValue(terminalLineDto.Id, out bool value))
                {
                    matched = value;
                }
                else
                {
                    matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterCondition);
                    lineFilterDict.Add(terminalLineDto.Id, matched);
                    if (matched)
                    {
                        shownLines.Add(terminalLineDto);
                    }
                }

                if (matched)
                {
                    AddTerminalLine(terminalLineDto);
                }
            }
        }

        private void Supervisor_TerminalLinesAdded(object sender, TerminalLineDtosEventArgs e)
        {
            AddNewTerminalLines(e.TerminalLines);
        }

        private void Supervisor_TerminalLinesRemoved(object sender, TerminalLineDtosEventArgs e)
        {
            FilterTerminal();
        }

        public ITerminalSupervisor? TerminalLineSupervisor
        {
            get => terminalLineSupervisor;
            set
            {
                if (terminalLineSupervisor != value && terminalLineSupervisor != null)
                {
                    terminalLineSupervisor.TerminalLinesAdded -= Supervisor_TerminalLinesAdded;
                    terminalLineSupervisor.TerminalLinesRemoved -= Supervisor_TerminalLinesRemoved;
                }

                terminalLineSupervisor = value;

                if (terminalLineSupervisor != null)
                {
                    terminalLineSupervisor.TerminalLinesAdded += Supervisor_TerminalLinesAdded;
                    terminalLineSupervisor.TerminalLinesRemoved += Supervisor_TerminalLinesRemoved;
                }
            }
        }

        public List<FieldDisplayDetail> VisibleFields
        {
            get => fieldListView.Fields;
            set
            {
                fieldListView.Fields = value;
                ApplyVisibleField();
            }
        }

        public IEnumerable<GridViewColumnSetting> ColumnSettings
        {
            get => columnSettingHelper.GetGridViewColumnSettings();
            set => columnSettingHelper.SetGridViewColumnSettings(value);
        }

        public GroupCondition FilterCondition
        {
            get => filterConditionListView.Condition;
            set
            {
                filterConditionListView.Condition = value;
                FilterTerminal();
            }
        }

        public GroupCondition FindCondition
        {
            get => findConditionListView.Condition;
            set => findConditionListView.Condition = value;
        }

        public ItemClipboard<FieldDisplayDetail>? FieldClipboard
        {
            get => fieldListView.FieldClipboard;
            set => fieldListView.FieldClipboard = value;
        }

        public ItemClipboard<TextStyleCondition>? StyleConditionClipboard
        {
            get => fieldListView.StyleConditionClipboard;
            set => fieldListView.StyleConditionClipboard = value;
        }

        public ItemClipboard<Condition>? FilterListClipboard
        {
            get => filterConditionListView.ConditionListClipboard;
            set => filterConditionListView.ConditionListClipboard = value;
        }

        public ItemClipboard<Condition>? FilterTreeClipboard
        {
            get => filterConditionListView.ConditionTreeClipboard;
            set => filterConditionListView.ConditionTreeClipboard = value;
        }

        public ItemClipboard<Condition>? FindListClipboard
        {
            get => findConditionListView.ConditionListClipboard;
            set => findConditionListView.ConditionListClipboard = value;
        }

        public ItemClipboard<Condition>? FindTreeClipboard
        {
            get => findConditionListView.ConditionTreeClipboard;
            set => findConditionListView.ConditionTreeClipboard = value;
        }
    }
}
