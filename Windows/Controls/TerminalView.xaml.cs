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
        private const string defaultColumn = "PlainText";

        private readonly List<TerminalLineVO> terminalLineVOs = new();

        private readonly DataTable terminalDataTable = new();

        private DispatcherTimer timer;

        private IEnumerable<FieldStyleCondition> fieldStyleConditions;
        private IEnumerable<string> visibleFieldKeys = new List<string>();
        private IEnumerable<FilterCondition> filterConditions = new List<FilterCondition>(0);

        private readonly TerminalViewDataContextVO dataContextVO = new();

        public TerminalView()
        {
            InitializeComponent();

            DataContext = dataContextVO;
            ApplyVisibleField();

            fieldStyleConditions = new FieldStyleCondition[] {
                new FieldStyleCondition
                {
                    FieldKey = "time",
                    Conditions = new TextStyleCondition[]{
                        new TextStyleCondition()
                        {
                            Style = new TextStyle()
                            {
                                Foreground = Colors.Green,
                                Background = Colors.Red,
                            },
                            Condition = new TextCondition(){
                                FieldKey = "time",
                                MatchOperator = TextMatcher.MatchOperator.Contains,
                                TargetValue = "05",
                            }
                        },
                        new TextStyleCondition()
                        {
                            Style = new TextStyle()
                            {
                                Foreground = Colors.Blue,
                                Background = Colors.Yellow,
                            },
                            Condition = new TextCondition(){
                                FieldKey = "time",
                                MatchOperator = TextMatcher.MatchOperator.Contains,
                                TargetValue = "03",
                            }
                        },
                        new TextStyleCondition()                                       
                        {
                            Style = new TextStyle()
                            {
                                Foreground = Colors.Red,
                                Background = Colors.White
                            },
                            Condition = new TextCondition(){
                                FieldKey = "time",
                                MatchOperator = TextMatcher.MatchOperator.Contains,
                                TargetValue = "09",
                            }
                        },
                    },
                    DefaultStyle = new TextStyle
                    {
                        Foreground = Colors.Black,
                        Background = Colors.White,
                    }
                }
            };
        }

        private void ButtonApplyFields_Click(object sender, RoutedEventArgs e)
        {
            PauseTimer();

            //visibleFieldKeys = fieldListView.FieldKeys.ToArray();
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

        public void AddExecution(IExecution execution)
        {
            StartTimer(execution);
        }

        private void StartTimer(IExecution execution)
        {
            timer = new();
            timer.Tick += (sender, e) =>
            {
                var lines = execution.ReadTerminalLines();
                foreach (var line in lines)
                {
                    ParseTerminalLine(line);
                }

                if (execution.IsCompleted)
                {
                    timer.Stop();
                    ParseTerminalLine("Task is completed");
                }

            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void PauseTimer()
        {
            timer.Stop();
        }

        private void ResumeTimer()
        {
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
            if (visibleFieldKeys.Any())
            {
                foreach (var visibleFieldKey in visibleFieldKeys)
                {
                    DataColumn column = new(visibleFieldKey);
                    column.DataType = typeof(string);
                    terminalDataTable.Columns.Add(column);

                    var visibleFieldStyleCondtion = fieldStyleConditions.FirstOrDefault(
                        feildStyelCondtion => feildStyelCondtion.FieldKey == visibleFieldKey);
                    if (visibleFieldStyleCondtion != null)
                    {
                        DataColumn foregroundColumn = new(GetForegroundColumnName(visibleFieldKey));
                        foregroundColumn.DataType = typeof(Brush);
                        terminalDataTable.Columns.Add(foregroundColumn);

                        DataColumn backgroundColumn = new(GetBackgroundColumnName(visibleFieldKey));
                        backgroundColumn.DataType = typeof(Brush);
                        terminalDataTable.Columns.Add(backgroundColumn);

                        FrameworkElementFactory fef = new(typeof(TextBlock));
                        Binding textBinding = new();
                        textBinding.Path = new PropertyPath(visibleFieldKey, Array.Empty<object>());
                        fef.SetBinding(TextBlock.TextProperty, textBinding);
                        Binding foregroundBinding = new();
                        foregroundBinding.Path = new PropertyPath(GetForegroundColumnName(visibleFieldKey), Array.Empty<object>());
                        fef.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                        Binding backgroundBinding = new();
                        backgroundBinding.Path = new PropertyPath(GetBackgroundColumnName(visibleFieldKey), Array.Empty<object>());
                        fef.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);
                        DataTemplate dataTemplate = new();
                        dataTemplate.VisualTree = fef;

                        gridView.Columns.Add(new GridViewColumn()
                        {
                            Header = visibleFieldKey,
                            CellTemplate = dataTemplate,
                        });
                    }
                    else
                    {
                        gridView.Columns.Add(new GridViewColumn()
                        {
                            Header = visibleFieldKey,
                            DisplayMemberBinding = new Binding(visibleFieldKey),
                        });
                    }
                }
            }
            else
            {
                DataColumn column = new(defaultColumn);
                column.DataType = typeof(string);
                terminalDataTable.Columns.Add(column);

                gridView.Columns.Add(new GridViewColumn()
                {
                    Header = defaultColumn,
                    DisplayMemberBinding = new Binding(defaultColumn),
                });
            }

            AddMatchedTerminalLines();

            listTerminal.View = gridView;
            Binding binding = new();
            listTerminal.DataContext = terminalDataTable;
            listTerminal.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private void AddTerminalLine(TerminalLineVO terminalLineVO)
        {
            DataRow row = terminalDataTable.NewRow();

            if (visibleFieldKeys.Any())
            {
                foreach (var visibleFieldKey in visibleFieldKeys)
                {
                    var fieldValue = terminalLineVO.ParsedFieldDict.ContainsKey(visibleFieldKey) ?
                    terminalLineVO.ParsedFieldDict[visibleFieldKey] : "";

                    row[visibleFieldKey] = fieldValue;

                    var visibleFieldStyleCondtion = fieldStyleConditions.FirstOrDefault(
                        fieldStyleCondition => fieldStyleCondition.FieldKey == visibleFieldKey);

                    if (visibleFieldStyleCondtion != null)
                    {
                        var matchedTextStyleCondition = visibleFieldStyleCondtion.Conditions.FirstOrDefault(
                            textStyleCondition => TerminalLineMatcher.IsMatch(terminalLineVO, textStyleCondition.Condition));
                        var textStyle = matchedTextStyleCondition?.Style ?? visibleFieldStyleCondtion.DefaultStyle;

                        row[GetForegroundColumnName(visibleFieldKey)] = new SolidColorBrush(textStyle.Foreground);
                        row[GetBackgroundColumnName(visibleFieldKey)] = new SolidColorBrush(textStyle.Background);
                    }
                }              
            }
            else
            {
                row[defaultColumn] = terminalLineVO.PlainText;
            }

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
    }
}
