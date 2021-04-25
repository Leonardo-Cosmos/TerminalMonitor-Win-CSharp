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
using TerminalMonitor.Checkers;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FilterListView.xaml
    /// </summary>
    public partial class FilterListView : UserControl
    {
        private readonly FilterItemVO currentFilter = new();
        private readonly ObservableCollection<FilterItemVO> filters = new();

        public FilterListView()
        {
            InitializeComponent();

            DataContext = currentFilter;
            cmbBxOperator.ItemsSource = Enum.GetValues(typeof(TextChecker.CheckOperator));
            lstFilters.ItemsSource = filters;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            FilterItemVO item = new()
            {
                FieldKey = currentFilter.FieldKey,
                CheckOperator = currentFilter.CheckOperator,
                ComparedValue = currentFilter.ComparedValue,
            };
            filters.Add(item);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                selectedItem.FieldKey = currentFilter.FieldKey;
                selectedItem.CheckOperator = currentFilter.CheckOperator;
                selectedItem.ComparedValue = currentFilter.ComparedValue;
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
                currentFilter.CheckOperator = selectedItem.CheckOperator;
                currentFilter.ComparedValue = selectedItem.ComparedValue;
            } else
            {
                currentFilter.FieldKey = String.Empty;
                currentFilter.CheckOperator = TextChecker.CheckOperator.None;
                currentFilter.ComparedValue = String.Empty;
            }
        }
    }
}
