/* 2021/5/9 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
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
using TerminalMonitor.Windows.Converters;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalView.xaml
    /// </summary>
    public partial class TerminalView : UserControl
    {
        private ITerminalLineSupervisor lineSupervisor;

        private const string idColumnName = "Id";

        private const string plaintextColumnName = "PlainText";

        private readonly DataTable terminalDataTable = new();

        private List<FieldDisplayDetail> visibleFields = new();

        private readonly TerminalViewColumnSettingHelper columnSettingHelper = new();

        private GroupCondition filterCondition = new();

        private GroupCondition findCondition = new();

        private readonly TerminalViewDataContextVO dataContextVO = new();

        private readonly Dictionary<string, bool> shownLineDict = new();

        private readonly List<TerminalLineDto> shownLines = new();

        private readonly List<(TerminalLineDto terminalLine, int shownIndex)> foundLines = new();

        public TerminalView()
        {
            InitializeComponent();

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

        private void MenuItemShowDetail_Click(object sender, RoutedEventArgs e)
        {
            if (listTerminal.SelectedValue is DataRowView item)
            {
                var id = (string)item[idColumnName];
                var terminalLine = lineSupervisor.TerminalLines[id];

                TerminalLineDetailWindow window = new()
                {
                    TerminalLine = terminalLine,
                };
                window.Show();
            }
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTerminal();
        }

        private void MenuItemAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            dataContextVO.AutoScroll = !dataContextVO.AutoScroll;
        }

        private void ClearTerminal()
        {
            terminalDataTable.Rows.Clear();
        }

        public void AddNewTerminalLines(IEnumerable<TerminalLineDto> terminalLineDtos)
        {
            var isAnyAdded = false;
            foreach (var terminalLineDto in terminalLineDtos)
            {
                var matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterCondition);
                shownLineDict.Add(terminalLineDto.Id, matched);
                shownLines.Add(terminalLineDto);

                if (matched)
                {
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

            if (lineSupervisor == null)
            {
                return;
            }

            terminalDataTable.Rows.Clear();

            shownLineDict.Clear();
            shownLines.Clear();
            TerminalLineMatcher matcher = new(filterCondition);
            foreach (var terminalLineDto in lineSupervisor.TerminalLines)
            {
                var matched = matcher.IsMatch(terminalLineDto);
                shownLineDict.Add(terminalLineDto.Id, matched);
                shownLines.Add(terminalLineDto);
            }

            AddMatchedTerminalLines();
        }

        private void FindInTerminal()
        {
            findCondition = (GroupCondition)findConditionListView.Condition.Clone();

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
        }

        private void FindPrevious()
        {
            if (foundLines.Count == 0)
            {
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            (TerminalLineDto terminalLine, int shownIndex) lineTuple;
            if (selectedIndex == -1)
            {
                lineTuple = foundLines.Last();
            }
            else
            {
                lineTuple = foundLines.AsEnumerable().Reverse().FirstOrDefault(tuple => tuple.shownIndex < selectedIndex);
                if (lineTuple == default)
                {
                    lineTuple = foundLines.Last();
                }
            }

            listTerminal.SelectedIndex = lineTuple.shownIndex;
            listTerminal.ScrollIntoView(lineTuple.terminalLine);
        }

        private void FindNext()
        {
            if (foundLines.Count == 0)
            {
                return;
            }

            var selectedIndex = listTerminal.SelectedIndex;

            (TerminalLineDto terminalLine, int shownIndex) lineTuple;
            if (selectedIndex == -1)
            {
                lineTuple = foundLines.First();
            }
            else
            {
                lineTuple = foundLines.FirstOrDefault(tuple => tuple.shownIndex > selectedIndex);
                if (lineTuple == default)
                {
                    lineTuple = foundLines.First();
                }
            }

            listTerminal.SelectedIndex = lineTuple.shownIndex;
            listTerminal.ScrollIntoView(lineTuple.terminalLine);
        }

        private void ApplyVisibleField()
        {
            // Save current column settings before reload grid.
            var gridViewColumnSettings = columnSettingHelper.GetGridViewColumnSettings();

            visibleFields = columnSettingHelper.Init(fieldListView.Fields);

            GridView gridView = new();

            terminalDataTable.Columns.Clear();
            terminalDataTable.Rows.Clear();

            DataColumn idColumn = new(idColumnName);
            idColumn.DataType = typeof(string);
            terminalDataTable.Columns.Add(idColumn);

            if (visibleFields.Any())
            {
                /*
                 * Add selected visible fields.
                 */
                foreach (var visibleField in visibleFields)
                {
                    DataColumn column = new(visibleField.Id);
                    column.DataType = typeof(string);
                    terminalDataTable.Columns.Add(column);

                    GridViewColumn viewColumn;
                    if (visibleField.CustomizeStyle)
                    {
                        DataTemplate dataTemplate = TerminalViewHelper.BuildFieldDataTemplate(visibleField, terminalDataTable);

                        viewColumn = new()
                        {
                            Header = visibleField.FieldKey,
                            CellTemplate = dataTemplate,
                        };
                    }
                    else
                    {
                        viewColumn = new()
                        {
                            Header = visibleField.FieldKey,
                            DisplayMemberBinding = new Binding(visibleField.Id),
                        };
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
                DataColumn defaultColumn = new(plaintextColumnName);
                defaultColumn.DataType = typeof(string);
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

        private void AddTerminalLine(TerminalLineDto terminalLineDto)
        {
            DataRow row = terminalDataTable.NewRow();

            row[idColumnName] = terminalLineDto.Id;

            if (visibleFields.Any())
            {
                foreach (var visibleField in visibleFields)
                {
                    var fieldValue = terminalLineDto.LineFieldDict.ContainsKey(visibleField.FieldKey) ?
                        terminalLineDto.LineFieldDict[visibleField.FieldKey].Text : "";

                    row[visibleField.Id] = fieldValue;

                    if (visibleField.CustomizeStyle)
                    {
                        var matchedTextStyleCondition = visibleField.Conditions.FirstOrDefault(
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
            if (lineSupervisor == null)
            {
                return;
            }

            foreach (var terminalLineDto in lineSupervisor.TerminalLines)
            {
                bool matched;
                if (shownLineDict.ContainsKey(terminalLineDto.Id))
                {
                    matched = shownLineDict[terminalLineDto.Id];
                }
                else
                {
                    matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterCondition);
                    shownLineDict.Add(terminalLineDto.Id, matched);
                    shownLines.Add(terminalLineDto);
                }

                if (matched)
                {
                    AddTerminalLine(terminalLineDto);
                }
            }
        }

        private void Supervisor_TerminalLinesAdded(object sender, TerminalLinesEventArgs e)
        {
            AddNewTerminalLines(e.TerminalLines);
        }

        public ITerminalLineSupervisor LineSupervisor
        {
            get => lineSupervisor;
            set
            {
                if (lineSupervisor != value && lineSupervisor != null)
                {
                    lineSupervisor.TerminalLinesAdded -= Supervisor_TerminalLinesAdded;
                }

                lineSupervisor = value;

                if (lineSupervisor != null)
                {
                    lineSupervisor.TerminalLinesAdded += Supervisor_TerminalLinesAdded;
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

        public ItemClipboard<FieldDisplayDetail> FieldClipboard
        {
            get => fieldListView.FieldClipboard;
            set => fieldListView.FieldClipboard = value;
        }

        public ItemClipboard<TextStyleCondition> StyleConditionClipboard
        {
            get => fieldListView.StyleConditionClipboard;
            set => fieldListView.StyleConditionClipboard = value;
        }

        public ItemClipboard<Condition> FilterListClipboard
        {
            get => filterConditionListView.ConditionListClipboard;
            set => filterConditionListView.ConditionListClipboard = value;
        }

        public ItemClipboard<Condition> FilterTreeClipboard
        {
            get => filterConditionListView.ConditionTreeClipboard;
            set => filterConditionListView.ConditionTreeClipboard = value;
        }

        public ItemClipboard<Condition> FindListClipboard
        {
            get => findConditionListView.ConditionListClipboard;
            set => findConditionListView.ConditionListClipboard = value;
        }

        public ItemClipboard<Condition> FindTreeClipboard
        {
            get => findConditionListView.ConditionTreeClipboard;
            set => findConditionListView.ConditionTreeClipboard = value;
        }
    }
}
