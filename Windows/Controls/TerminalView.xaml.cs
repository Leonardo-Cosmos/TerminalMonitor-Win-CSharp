/* 2021/5/9 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private IEnumerable<FieldDisplayDetail> visibleFields = Array.Empty<FieldDisplayDetail>();
        private GroupCondition filterCondition = new();

        private readonly TerminalViewDataContextVO dataContextVO = new();

        private readonly Dictionary<string, bool> matchedLineDict = new();

        private static readonly IntToHorizontalAlignmentConverter horizontalAlignmentConverter = new();

        private static readonly IntToVerticalAlignmentConverter verticalAlignmentConverter = new();

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
            visibleFields = fieldListView.Fields.ToArray();
            ApplyVisibleField();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            filterCondition = filterView.Condition;
            FilterTerminal();
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

        public void AddNewTerminalLine(TerminalLineDto terminalLineDto)
        {
            var matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterCondition);
            matchedLineDict.Add(terminalLineDto.Id, matched);

            if (matched)
            {
                AddTerminalLine(terminalLineDto);

                if (dataContextVO.AutoScroll)
                {
                    var itemCount = listTerminal.Items.Count;
                    var lastItem = listTerminal.Items[itemCount - 1];
                    listTerminal.ScrollIntoView(lastItem);
                }
            }
        }

        private void FilterTerminal()
        {
            if (lineSupervisor == null)
            {
                return;
            }

            terminalDataTable.Rows.Clear();

            matchedLineDict.Clear();
            TerminalLineMatcher matcher = new(filterCondition);
            foreach (var terminalLineDto in lineSupervisor.TerminalLines)
            {
                var matched = matcher.IsMatch(terminalLineDto);
                matchedLineDict.Add(terminalLineDto.Id, matched);
            }

            AddMatchedTerminalLines();
        }

        private static string GetForegroundColumnName(string columnName)
        {
            return $"{columnName!}__foreground";
        }

        private static string GetBackgroundColumnName(string columnName)
        {
            return $"{columnName!}__background";
        }

        private static string GetTextHorizontalAlignmentColumnName(string columnName)
        {
            return $"{columnName!}__textHorizontalAlignment";
        }

        private static string GetTextVertialAlignmentColumnName(string columnName)
        {
            return $"{columnName!}__textVerticalAlignment";
        }

        private void ApplyVisibleField()
        {
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

                    if (visibleField.CustomizeStyle)
                    {
                        DataTemplate dataTemplate = BuildFieldDataTemplate(visibleField, terminalDataTable);

                        gridView.Columns.Add(new GridViewColumn()
                        {
                            Header = visibleField.FieldKey,
                            CellTemplate = dataTemplate,
                        });
                    }
                    else
                    {
                        gridView.Columns.Add(new GridViewColumn()
                        {
                            Header = visibleField.FieldKey,
                            DisplayMemberBinding = new Binding(visibleField.Id),
                        });
                    }
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

            AddMatchedTerminalLines();

            listTerminal.View = gridView;
            Binding binding = new();
            listTerminal.DataContext = terminalDataTable;
            listTerminal.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private static DataTemplate BuildFieldDataTemplate(FieldDisplayDetail visibleField, DataTable terminalDataTable)
        {
            DataColumn foregroundColumn = new(GetForegroundColumnName(visibleField.Id));
            foregroundColumn.DataType = typeof(Brush);
            terminalDataTable.Columns.Add(foregroundColumn);

            DataColumn backgroundColumn = new(GetBackgroundColumnName(visibleField.Id));
            backgroundColumn.DataType = typeof(Brush);
            terminalDataTable.Columns.Add(backgroundColumn);

            DataColumn textHorizontalAlignmentColumn = new(GetTextHorizontalAlignmentColumnName(visibleField.Id));
            textHorizontalAlignmentColumn.DataType = typeof(HorizontalAlignment);
            terminalDataTable.Columns.Add(textHorizontalAlignmentColumn);

            DataColumn textVerticalAlignmentColumn = new(GetTextVertialAlignmentColumnName(visibleField.Id));
            textVerticalAlignmentColumn.DataType = typeof(VerticalAlignment);
            terminalDataTable.Columns.Add(textVerticalAlignmentColumn);

            FrameworkElementFactory textBlockElement = new(typeof(TextBlock));

            Binding textBinding = new();
            textBinding.Path = new PropertyPath(visibleField.Id, Array.Empty<object>());
            textBlockElement.SetBinding(TextBlock.TextProperty, textBinding);

            Binding foregroundBinding = new();
            foregroundBinding.Path = new PropertyPath(GetForegroundColumnName(visibleField.Id), Array.Empty<object>());
            textBlockElement.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);

            Binding backgroundBinding = new();
            backgroundBinding.Path = new PropertyPath(GetBackgroundColumnName(visibleField.Id), Array.Empty<object>());
            textBlockElement.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);

            Binding textHorizontalAlignmentBinding = new();
            textHorizontalAlignmentBinding.Path = 
                new PropertyPath(GetTextHorizontalAlignmentColumnName(visibleField.Id), Array.Empty<object>());
            textHorizontalAlignmentBinding.Converter = horizontalAlignmentConverter;
            textBlockElement.SetBinding(TextBlock.HorizontalAlignmentProperty, textHorizontalAlignmentBinding);

            Binding textVerticalAlignmentBinding = new();
            textVerticalAlignmentBinding.Path =
                new PropertyPath(GetTextVertialAlignmentColumnName(visibleField.Id), Array.Empty<object>());
            textVerticalAlignmentBinding.Converter = verticalAlignmentConverter;
            textBlockElement.SetBinding(TextBlock.VerticalAlignmentProperty, textVerticalAlignmentBinding);

            FrameworkElementFactory panelElement = new(typeof(DockPanel));
            panelElement.SetValue(DockPanel.BackgroundProperty, Brushes.Black);
            panelElement.AppendChild(textBlockElement);

            DataTemplate dataTemplate = new();
            dataTemplate.VisualTree = panelElement;
            return dataTemplate;
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
                        var textStyle = matchedTextStyleCondition?.Style ?? visibleField.Style;

                        row[GetForegroundColumnName(visibleField.Id)] = new SolidColorBrush(textStyle.Foreground);
                        row[GetBackgroundColumnName(visibleField.Id)] = new SolidColorBrush(textStyle.Background);
                        row[GetTextHorizontalAlignmentColumnName(visibleField.Id)] = HorizontalAlignment.Center;
                        row[GetTextVertialAlignmentColumnName(visibleField.Id)] = VerticalAlignment.Center;
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
                if (matchedLineDict.ContainsKey(terminalLineDto.Id))
                {
                    matched = matchedLineDict[terminalLineDto.Id];
                }
                else
                {
                    matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterCondition);
                    matchedLineDict.Add(terminalLineDto.Id, matched);
                }

                if (matched)
                {
                    AddTerminalLine(terminalLineDto);
                }
            }
        }

        private void Supervisor_TerminalLineAdded(object sender, TerminalLineEventArgs e)
        {
            AddNewTerminalLine(e.TerminalLine);
        }

        public ITerminalLineSupervisor LineSupervisor
        {
            get => lineSupervisor;
            set
            {
                if (lineSupervisor != value && lineSupervisor != null)
                {
                    lineSupervisor.TerminalLineAdded -= Supervisor_TerminalLineAdded;
                }

                lineSupervisor = value;

                if (lineSupervisor != null)
                {
                    lineSupervisor.TerminalLineAdded += Supervisor_TerminalLineAdded;
                }
            }
        }

        public IEnumerable<FieldDisplayDetail> VisibleFields
        {
            get
            {
                List<FieldDisplayDetail> fieldList = new();
                fieldList.AddRange(visibleFields);
                return new ReadOnlyCollection<FieldDisplayDetail>(fieldList);
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                fieldListView.Fields = value;
                visibleFields = value.ToArray();
                ApplyVisibleField();
            }
        }

        public GroupCondition FilterCondition
        {
            get
            {
                return filterCondition;
            }

            set
            {
                filterCondition = value ?? new GroupCondition();
                filterView.Condition = filterCondition;
                FilterTerminal();
            }
        }

        public ItemClipboard<FieldDisplayDetail> FieldClipboard
        {
            get
            {
                return fieldListView.FieldClipboard;
            }

            set
            {
                fieldListView.FieldClipboard = value;
            }
        }

        public ItemClipboard<Condition> FilterClipboard
        {
            get
            {
                return filterView.FilterClipboard;
            }

            set
            {
                filterView.FilterClipboard = value;
            }
        }
    }
}
