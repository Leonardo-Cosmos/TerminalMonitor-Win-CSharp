/* 2021/4/21 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TerminalMonitor.Matchers;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FilterListView.xaml
    /// </summary>
    public partial class FilterListView : UserControl
    {
        private readonly ObservableCollection<FilterItemVO> filterVOs = new();

        private readonly List<Condition> conditions;

        private readonly ConditionGroup conditionGroup;

        public FilterListView()
        {
            InitializeComponent();

            conditions = new List<Condition>();
            conditionGroup = new()
            {
                MatchMode = GroupMatchMode.All,
                Conditions = conditions,
            };

            lstFilters.ItemsSource = filterVOs;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConditionDetailWindow window = new();

            if (window.ShowDialog() ?? false)
            {
                FilterItemVO item;
                Condition condition = window.Condition;
                if (!String.IsNullOrEmpty(condition.Name))
                {
                    item = new()
                    {
                        ConditionName = condition.Name,
                    };
                }
                else if (condition is FieldCondition fieldCondition)
                {
                    item = new()
                    {
                        FieldKey = fieldCondition.FieldKey,
                        MatchOperator = fieldCondition.MatchOperator,
                        TargetValue = fieldCondition.TargetValue,
                    };
                }
                else
                {
                    throw new NotImplementedException("Condition without name or field");
                }

                filterVOs.Add(item);
                lstFilters.SelectedItem = item;

                conditions.Add(condition);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                var index = filterVOs.IndexOf(selectedItem);
                ConditionDetailWindow window = new()
                {
                    Condition = conditions[index],
                };

                if (window.ShowDialog() ?? false)
                {
                    FilterItemVO item;
                    Condition condition = window.Condition;
                    if (!String.IsNullOrEmpty(condition.Name))
                    {
                        item = new()
                        {
                            ConditionName = condition.Name,
                        };
                    }
                    else if (condition is FieldCondition fieldCondition)
                    {
                        item = new()
                        {
                            FieldKey = fieldCondition.FieldKey,
                            MatchOperator = fieldCondition.MatchOperator,
                            TargetValue = fieldCondition.TargetValue,
                        };
                    }
                    else
                    {
                        throw new NotImplementedException("Condition without name or field");
                    }

                    filterVOs[index] = item;
                    conditions[index] = condition;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedValue is FilterItemVO selectedItem)
            {
                var index = filterVOs.IndexOf(selectedItem);
                filterVOs.RemoveAt(index);
                conditions.RemoveAt(index);
            }
        }

        internal IEnumerable<FilterCondition> FilterConditions
        {
            get
            {
                return filterVOs.Select(filterVO => new FilterCondition()
                {
                    Condition = new FieldCondition()
                    {
                        FieldKey = filterVO.FieldKey,
                        MatchOperator = filterVO.MatchOperator,
                        TargetValue = filterVO.TargetValue,
                    },

                    Excluded = false,
                });
            }

            set
            {
                filterVOs.Clear();
                if (value == null)
                {
                    return;
                }
                value.Select(filter => new FilterItemVO()
                {
                    FieldKey = filter.Condition?.FieldKey,
                    MatchOperator = filter.Condition?.MatchOperator ?? TextMatchOperator.None,
                    TargetValue = filter.Condition?.TargetValue,
                }).ToList().ForEach(filterVO => filterVOs.Add(filterVO));
            }
        }
    }
}
