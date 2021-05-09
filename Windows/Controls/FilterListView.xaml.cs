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
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FilterListView.xaml
    /// </summary>
    public partial class FilterListView : UserControl
    {
        private readonly FilterItemVO currentFilter = new();
        private readonly ObservableCollection<FilterItemVO> filters = new();

        internal IReadOnlyList<FilterCondition> FilterConditions
        {
            get
            {
                return filters.Select(filterVo => new FilterCondition()
                {
                    Condition = new TextCondition()
                    {
                        FieldKey = filterVo.FieldKey,
                        MatchOperator = filterVo.MatchOperator,
                        TargetValue = filterVo.TargetValue,
                    },

                    Excluded = false,
                }).ToList().AsReadOnly();
            }

            set
            {
                filters.Clear();
                if (value == null)
                {
                    return;
                }
                value.Select(filter => new FilterItemVO()
                {
                    FieldKey = filter.Condition?.FieldKey,
                    MatchOperator = filter.Condition?.MatchOperator ?? TextMatcher.MatchOperator.None,
                    TargetValue = filter.Condition?.TargetValue,
                }).ToList().ForEach(filterVO => filters.Add(filterVO));
            }
        }

        public FilterListView()
        {
            InitializeComponent();

            DataContext = currentFilter;
            cmbBxOperator.ItemsSource = Enum.GetValues(typeof(TextMatcher.MatchOperator));
            lstFilters.ItemsSource = filters;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            FilterItemVO item = new()
            {
                FieldKey = currentFilter.FieldKey,
                MatchOperator = currentFilter.MatchOperator,
                TargetValue = currentFilter.TargetValue,
            };
            filters.Add(item);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                selectedItem.FieldKey = currentFilter.FieldKey;
                selectedItem.MatchOperator = currentFilter.MatchOperator;
                selectedItem.TargetValue = currentFilter.TargetValue;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedValue is FilterItemVO selectedItem)
            {
                filters.Remove(selectedItem);
            }
        }

        private void LstFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                currentFilter.FieldKey = selectedItem.FieldKey;
                currentFilter.MatchOperator = selectedItem.MatchOperator;
                currentFilter.TargetValue = selectedItem.TargetValue;
            } else
            {
                currentFilter.FieldKey = String.Empty;
                currentFilter.MatchOperator = TextMatcher.MatchOperator.None;
                currentFilter.TargetValue = String.Empty;
            }
        }
    }
}
