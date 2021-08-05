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
        private readonly FilterListViewDataContextVO dataContextVO = new();

        private readonly ObservableCollection<FilterItemVO> filterVOs = new();

        private readonly List<Condition> conditions = new();

        public FilterListView()
        {
            InitializeComponent();

            DataContext = dataContextVO;

            lstFilters.ItemsSource = filterVOs;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConditionDetailWindow window = new();

            if (window.ShowDialog() ?? false)
            {
                Condition condition = window.Condition;

                FilterItemVO item = CreateFilterVO(condition);
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
                    Condition condition = window.Condition;

                    FilterItemVO item = CreateFilterVO(condition);
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

        private static FilterItemVO CreateFilterVO(Condition condition)
        {
            FilterItemVO item;
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

            item.NegativeMatch = condition.NegativeMatch;
            item.DefaultMatch = condition.DefaultMatch;
            item.DismissMatch = condition.DismissMatch;

            return item;
        }

        internal ConditionGroup ConditionGroup
        {
            get
            {
                return new ConditionGroup()
                {
                    MatchMode = dataContextVO.MatchMode,
                    NegativeMatch = dataContextVO.NegativeMatch,
                    DefaultMatch = dataContextVO.DefaultMatch,
                    DismissMatch = dataContextVO.DismissMatch,
                    Conditions = conditions,
                };
            }

            set
            {
                filterVOs.Clear();
                conditions.Clear();

                if (value == null)
                {
                    return;
                }

                dataContextVO.MatchMode = value.MatchMode;
                dataContextVO.NegativeMatch = value.NegativeMatch;
                dataContextVO.DefaultMatch = value.DefaultMatch;
                dataContextVO.DismissMatch = value.DismissMatch;
                if (value.Conditions != null)
                {
                    foreach (Condition condition in value.Conditions)
                    {
                        conditions.Add(condition);
                        filterVOs.Add(CreateFilterVO(condition));
                    }
                }
            }
        }
    }
}
