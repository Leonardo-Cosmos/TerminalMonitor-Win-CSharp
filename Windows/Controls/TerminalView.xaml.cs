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
using TerminalMonitor.Execution;
using TerminalMonitor.Matchers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalView.xaml
    /// </summary>
    public partial class TerminalView : UserControl
    {
        private ITerminalLineSupervisor lineSupervisor;

        private const string defaultColumnName = "PlainText";

        private readonly DataTable terminalDataTable = new();

        private IEnumerable<FieldDisplayDetail> visibleFields = Array.Empty<FieldDisplayDetail>();
        private IEnumerable<FilterCondition> filterConditions = Array.Empty<FilterCondition>();

        private readonly TerminalViewDataContextVO dataContextVO = new();

        private readonly Dictionary<string, bool> matchedLineDict = new();

        public TerminalView()
        {
            InitializeComponent();

            DataContext = dataContextVO;
            ApplyVisibleField();
        }

        private void ButtonApplyFields_Click(object sender, RoutedEventArgs e)
        {
            //PauseTimer();

            visibleFields = fieldListView.Fields.ToArray();
            ApplyVisibleField();

            //ResumeTimer();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            //PauseTimer();

            filterConditions = filterView.FilterConditions.ToArray();
            FilterTerminal();

            //ResumeTimer();
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            //PauseTimer();

            ClearTerminal();

            //ResumeTimer();
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
            var matched = TerminalLineMatcher.IsMatch(terminalLineDto, filterConditions);
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

            matchedLineDict.Clear();
            TerminalLineMatcher matcher = new(filterConditions);
            foreach (var terminalLineDto in lineSupervisor.TerminalLines)
            {
                var matched = matcher.IsMatch(terminalLineDto);
                matchedLineDict.Add(terminalLineDto.Id, matched);
            }

            AddMatchedTerminalLines();
        }

        private static string GetForegroundColumnName(string columnName)
        {
            return $"{columnName}__foreground";
        }

        private static string GetBackgroundColumnName(string columnName)
        {
            return $"{columnName}__background";
        }

        private void ApplyVisibleField()
        {
            GridView gridView = new();

            terminalDataTable.Columns.Clear();
            terminalDataTable.Rows.Clear();
            if (visibleFields.Any())
            {
                foreach (var visibleField in visibleFields)
                {
                    DataColumn column = new(visibleField.FieldKey);
                    column.DataType = typeof(string);
                    terminalDataTable.Columns.Add(column);

                    if (visibleField.CustomizeStyle)
                    {
                        DataColumn foregroundColumn = new(GetForegroundColumnName(visibleField.FieldKey));
                        foregroundColumn.DataType = typeof(Brush);
                        terminalDataTable.Columns.Add(foregroundColumn);

                        DataColumn backgroundColumn = new(GetBackgroundColumnName(visibleField.FieldKey));
                        backgroundColumn.DataType = typeof(Brush);
                        terminalDataTable.Columns.Add(backgroundColumn);

                        FrameworkElementFactory fef = new(typeof(TextBlock));
                        Binding textBinding = new();
                        textBinding.Path = new PropertyPath(visibleField.FieldKey, Array.Empty<object>());
                        fef.SetBinding(TextBlock.TextProperty, textBinding);
                        Binding foregroundBinding = new();
                        foregroundBinding.Path = new PropertyPath(GetForegroundColumnName(visibleField.FieldKey), Array.Empty<object>());
                        fef.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                        Binding backgroundBinding = new();
                        backgroundBinding.Path = new PropertyPath(GetBackgroundColumnName(visibleField.FieldKey), Array.Empty<object>());
                        fef.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);
                        DataTemplate dataTemplate = new();
                        dataTemplate.VisualTree = fef;

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
                            DisplayMemberBinding = new Binding(visibleField.FieldKey),
                        });
                    }
                }
            }

            DataColumn defaultColumn = new(defaultColumnName);
            defaultColumn.DataType = typeof(string);
            terminalDataTable.Columns.Add(defaultColumn);

            gridView.Columns.Add(new GridViewColumn()
            {
                Header = defaultColumnName,
                DisplayMemberBinding = new Binding(defaultColumnName),
            });

            AddMatchedTerminalLines();

            listTerminal.View = gridView;
            Binding binding = new();
            listTerminal.DataContext = terminalDataTable;
            listTerminal.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private void AddTerminalLine(TerminalLineDto terminalLineDto)
        {
            DataRow row = terminalDataTable.NewRow();

            if (visibleFields.Any())
            {
                foreach (var visibleField in visibleFields)
                {
                    var fieldValue = terminalLineDto.JsonProperties.ContainsKey(visibleField.FieldKey) ?
                        terminalLineDto.JsonProperties[visibleField.FieldKey].Value : "";

                    row[visibleField.FieldKey] = fieldValue;

                    if (visibleField.CustomizeStyle)
                    {
                        var matchedTextStyleCondition = visibleField.Conditions.FirstOrDefault(
                            textStyleCondition => TerminalLineMatcher.IsMatch(terminalLineDto, textStyleCondition.Condition));
                        var textStyle = matchedTextStyleCondition?.Style ?? visibleField.Style;

                        row[GetForegroundColumnName(visibleField.FieldKey)] = new SolidColorBrush(textStyle.Foreground);
                        row[GetBackgroundColumnName(visibleField.FieldKey)] = new SolidColorBrush(textStyle.Background);
                    }
                }
            }

            row[defaultColumnName] = terminalLineDto.PlainText;

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
                var matched = matchedLineDict[terminalLineDto.Id];
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

                //PauseTimer();

                fieldListView.Fields = value;
                visibleFields = value.ToArray();
                ApplyVisibleField();

                //ResumeTimer();
            }
        }

        public IEnumerable<FilterCondition> FilterConditions
        {
            get
            {
                List<FilterCondition> filterList = new();
                filterList.AddRange(filterConditions);
                return new ReadOnlyCollection<FilterCondition>(filterList);
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                //PauseTimer();

                filterView.FilterConditions = value;
                filterConditions = value.ToArray();
                FilterTerminal();

                //ResumeTimer();
            }
        }
    }
}
