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
using TerminalMonitor.Parsers;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalView.xaml
    /// </summary>
    public partial class TerminalView : UserControl
    {
        private const string defaultColumnName = "PlainText";

        private readonly List<TerminalLineVO> terminalLineVOs = new();

        private readonly DataTable terminalDataTable = new();

        private DispatcherTimer timer;

        private IEnumerable<FieldDisplayDetail> visibleFields = Array.Empty<FieldDisplayDetail>();
        private IEnumerable<FilterCondition> filterConditions = Array.Empty<FilterCondition>();

        private readonly TerminalViewDataContextVO dataContextVO = new();

        public TerminalView()
        {
            InitializeComponent();

            DataContext = dataContextVO;
            ApplyVisibleField();
        }

        private void ButtonApplyFields_Click(object sender, RoutedEventArgs e)
        {
            PauseTimer();

            visibleFields = fieldListView.Fields.ToArray();
            ApplyVisibleField();

            ResumeTimer();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            PauseTimer();

            filterConditions = filterView.FilterConditions.ToArray();
            FilterTerminal();

            ResumeTimer();
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            PauseTimer();

            ClearTerminal();

            ResumeTimer();
        }

        private void MenuItemAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            dataContextVO.AutoScroll = !dataContextVO.AutoScroll;
        }

        public void AddExecution(ITerminalLineProducer producer)
        {
            StartTimer(producer);
        }

        private void StartTimer(ITerminalLineProducer producer)
        {
            timer = new();
            timer.Tick += (sender, e) =>
            {
                var lines = producer.ReadTerminalLines();
                foreach (var line in lines)
                {
                    ParseTerminalLine(line);
                }

                if (producer.IsCompleted)
                {
                    timer.Stop();
                }
            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void PauseTimer()
        {
            if (timer == null)
            {
                return;
            }
            timer.Stop();
        }

        private void ResumeTimer()
        {
            if (timer == null)
            {
                return;
            }
            timer.Start();
        }

        private void ClearTerminal()
        {
            terminalLineVOs.Clear();
            terminalDataTable.Rows.Clear();
        }

        private void ParseTerminalLine(string text)
        {
            var terminalLineVO = JsonParser.ParseTerminalLineToVO(text);
            terminalLineVOs.Add(terminalLineVO);


            terminalLineVO.Matched = TerminalLineMatcher.IsMatch(terminalLineVO, filterConditions);
            if (terminalLineVO.Matched)
            {
                AddTerminalLine(terminalLineVO);

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
            TerminalLineMatcher matcher = new (filterConditions);
            foreach (var terminalLineVO in terminalLineVOs)
            {
                terminalLineVO.Matched = matcher.IsMatch(terminalLineVO);
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

        private void AddTerminalLine(TerminalLineVO terminalLineVO)
        {
            DataRow row = terminalDataTable.NewRow();

            if (visibleFields.Any())
            {
                foreach (var visibleField in visibleFields)
                {
                    var fieldValue = terminalLineVO.ParsedFieldDict.ContainsKey(visibleField.FieldKey) ?
                    terminalLineVO.ParsedFieldDict[visibleField.FieldKey] : "";

                    row[visibleField.FieldKey] = fieldValue;

                    if (visibleField.CustomizeStyle)
                    {
                        var matchedTextStyleCondition = visibleField.Conditions.FirstOrDefault(
                            textStyleCondition => TerminalLineMatcher.IsMatch(terminalLineVO, textStyleCondition.Condition));
                        var textStyle = matchedTextStyleCondition?.Style ?? visibleField.Style;

                        row[GetForegroundColumnName(visibleField.FieldKey)] = new SolidColorBrush(textStyle.Foreground);
                        row[GetBackgroundColumnName(visibleField.FieldKey)] = new SolidColorBrush(textStyle.Background);
                    }
                }              
            }
            
            row[defaultColumnName] = terminalLineVO.PlainText;
 
            terminalDataTable.Rows.Add(row);
        }

        private void AddMatchedTerminalLines()
        {
            foreach (var terminalLineVO in terminalLineVOs)
            {
                if (terminalLineVO.Matched)
                {
                    AddTerminalLine(terminalLineVO);
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

                PauseTimer();

                fieldListView.Fields = value;
                visibleFields = value.ToArray();
                ApplyVisibleField();

                ResumeTimer();
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

                PauseTimer();

                filterView.FilterConditions = value;
                filterConditions = value.ToArray();
                FilterTerminal();

                ResumeTimer();
            }
        }
    }
}
