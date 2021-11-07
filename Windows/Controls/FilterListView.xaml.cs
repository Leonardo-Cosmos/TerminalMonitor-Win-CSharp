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
using TerminalMonitor.Clipboard;
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

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedValue is FilterItemVO selectedItem)
            {
                var index = filterVOs.IndexOf(selectedItem);
                filterVOs.RemoveAt(index);

                conditions.RemoveAt(index);
            }
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
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

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                var index = filterVOs.IndexOf(selectedItem);

                var condition = conditions[index];
                FilterClipboard?.Copy(condition);
            }
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            //var condition = FilterClipboard?.Paste();
            //if (condition != null)
            //{
            //    condition = (Condition)condition.Clone();

            //    FilterItemVO item = CreateFilterVO(condition);
            //    filterVOs.Add(item);
            //    lstFilters.SelectedItem = item;

            //    conditions.Add(condition);
            //}
        }

        private void BtnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                var srcIndex = filterVOs.IndexOf(selectedItem);
                var dstIndex = (srcIndex - 1 + filterVOs.Count) % filterVOs.Count;

                filterVOs.RemoveAt(srcIndex);
                filterVOs.Insert(dstIndex, selectedItem);

                lstFilters.SelectedItem = selectedItem;

                var condition = conditions[srcIndex];
                conditions.RemoveAt(srcIndex);
                conditions.Insert(dstIndex, condition);
            }
        }

        private void BtnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            if (lstFilters.SelectedItem is FilterItemVO selectedItem)
            {
                var srcIndex = filterVOs.IndexOf(selectedItem);
                var dstIndex = (srcIndex + 1) % filterVOs.Count;

                filterVOs.RemoveAt(srcIndex);
                filterVOs.Insert(dstIndex, selectedItem);

                lstFilters.SelectedItem = selectedItem;

                var condition = conditions[srcIndex];
                conditions.RemoveAt(srcIndex);
                conditions.Insert(dstIndex, condition);
            }
        }

        private static FilterItemVO CreateFilterVO(Condition condition)
        {
            FilterItemVO item;
            if (!String.IsNullOrEmpty(condition.Name))
            {
                item = new()
                {
                    Id = condition.Id,
                    ConditionName = condition.Name,
                };
            }
            else if (condition is FieldCondition fieldCondition)
            {
                item = new()
                {
                    Id = condition.Id,
                    FieldKey = fieldCondition.FieldKey,
                    MatchOperator = fieldCondition.MatchOperator,
                    TargetValue = fieldCondition.TargetValue,
                };
            }
            else
            {
                throw new NotImplementedException("Condition without name or field");
            }

            item.IsInverted = condition.IsInverted;
            item.DefaultResult = condition.DefaultResult;
            item.IsDisabled = condition.IsDisabled;

            return item;
        }

        public GroupCondition Condition
        {
            get
            {
                return new GroupCondition()
                {
                    MatchMode = dataContextVO.MatchMode,
                    IsInverted = dataContextVO.IsInverted,
                    DefaultResult = dataContextVO.DefaultResult,
                    IsDisabled = dataContextVO.IsDisabled,
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
                dataContextVO.IsInverted = value.IsInverted;
                dataContextVO.DefaultResult = value.DefaultResult;
                dataContextVO.IsDisabled = value.IsDisabled;
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

        public ItemClipboard<Condition> FilterClipboard
        {
            get; set;
        }
    }
}
