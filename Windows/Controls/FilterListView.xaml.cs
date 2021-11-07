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

        private List<Condition> conditions;

        private GroupCondition groupCondition;

        private ItemClipboard<Condition> conditionClipboard;

        public FilterListView()
        {
            InitializeComponent();

            conditions = new();
            groupCondition = new()
            {
                Id = Guid.NewGuid().ToString(),
                Conditions = conditions,
            };

            DataContext = dataContextVO;
            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;

            lstFilters.ItemsSource = filterVOs;
        }

        private void DataContextVO_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GroupCondition.MatchMode):
                    groupCondition.MatchMode = dataContextVO.MatchMode;
                    break;
                case nameof(GroupCondition.IsInverted):
                    groupCondition.IsInverted = dataContextVO.IsInverted;
                    break;
                case nameof(GroupCondition.DefaultResult):
                    groupCondition.DefaultResult = dataContextVO.DefaultResult;
                    break;
                case nameof(GroupCondition.IsDisabled):
                    groupCondition.IsDisabled = dataContextVO.IsDisabled;
                    break;
                case nameof(GroupCondition.Conditions):
                    groupCondition.Conditions = conditions; break;
                default:
                    break;
            }
        }

        private void LstFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstFilters.SelectedItems.Count;
            dataContextVO.IsAnySelected = count > 0;
        }

        private void LstFilters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ForSelectedItem(ModifyCondition);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCondition();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(DeleteCondition);
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(ModifyCondition);
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            CopyConditions();
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            PasteConditions();
        }

        private void BtnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(MoveConditionLeft, byOrder: true, recoverSelection: true);
        }

        private void BtnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(MoveConditionRight, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void ConditionClipboard_ItemCopied(object sender, EventArgs e)
        {
            dataContextVO.IsAnyConditionInClipboard = !conditionClipboard?.IsEmpty ?? false;
        }

        private void ConditionClipboard_ItemPasted(object sender, EventArgs e)
        {
            dataContextVO.IsAnyConditionInClipboard = !conditionClipboard?.IsEmpty ?? false;
        }

        private void ForSelectedItem(Action<FilterItemVO> action)
        {
            if (lstFilters.SelectedItem is FilterItemVO itemVO)
            {
                action(itemVO);
            }
        }

        private void ForEachSelectedItem(Action<FilterItemVO> action,
            bool byOrder = false, bool reverseOrder = false, bool recoverSelection = false)
        {
            List<FilterItemVO> itemVOs = new();
            foreach (var selectedItem in lstFilters.SelectedItems)
            {
                if (selectedItem is FilterItemVO itemVO)
                {
                    itemVOs.Add(itemVO);
                }
            }

            if (byOrder)
            {
                itemVOs.Sort((itemX, itemY) =>
                    filterVOs.IndexOf(itemX) - filterVOs.IndexOf(itemY));
            }

            if (reverseOrder)
            {
                itemVOs.Reverse();
            }

            itemVOs.ForEach(action);

            if (recoverSelection)
            {
                itemVOs.ForEach(itemVO => lstFilters.SelectedItems.Add(itemVO));
            }
        }

        private void AddCondition()
        {
            ConditionDetailWindow window = new();

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    var condition = window.Condition;

                    FilterItemVO item = CreateFilterVO(condition);
                    filterVOs.Add(item);
                    lstFilters.SelectedItem = item;

                    conditions.Add(condition);
                }
            };

            window.Show();
        }

        private void DeleteCondition(FilterItemVO itemVO)
        {
            var index = filterVOs.IndexOf(itemVO);
            filterVOs.RemoveAt(index);

            conditions.RemoveAt(index);
        }

        private void ModifyCondition(FilterItemVO itemVO)
        {
            var index = filterVOs.IndexOf(itemVO);

            var condition = conditions[index];
            ConditionDetailWindow window = new()
            {
                Condition = condition,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    var condition = window.Condition;

                    FilterItemVO item = CreateFilterVO(condition);
                    filterVOs[index] = item;

                    conditions[index] = condition;
                }
            };

            window.Show();
        }

        private void MoveConditionLeft(FilterItemVO itemVO)
        {
            var srcIndex = filterVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex - 1 + filterVOs.Count) % filterVOs.Count;

            filterVOs.RemoveAt(srcIndex);
            filterVOs.Insert(dstIndex, itemVO);

            var condition = conditions[srcIndex];
            conditions.RemoveAt(srcIndex);
            conditions.Insert(dstIndex, condition);
        }

        private void MoveConditionRight(FilterItemVO itemVO)
        {
            var srcIndex = filterVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex + 1) % filterVOs.Count;

            filterVOs.RemoveAt(srcIndex);
            filterVOs.Insert(dstIndex, itemVO);

            var condition = conditions[srcIndex];
            conditions.RemoveAt(srcIndex);
            conditions.Insert(dstIndex, condition);
        }

        private void CopyConditions()
        {
            List<Condition> selectedConditions = new();
            foreach (var selectedItem in lstFilters.SelectedItems)
            {
                if (lstFilters.SelectedItem is FilterItemVO itemVO)
                {
                    var index = filterVOs.IndexOf(itemVO);

                    var condition = conditions[index];
                    selectedConditions.Add(condition);
                }
            }

            conditionClipboard?.Copy(selectedConditions.ToArray());
        }

        private void PasteConditions()
        {
            var pastedConditions = conditionClipboard?.Paste();
            if (pastedConditions != null)
            {
                lstFilters.SelectedItems.Clear();
                foreach (var pastedCondition in pastedConditions)
                {
                    var condition = (Condition)pastedCondition.Clone();

                    FilterItemVO itemVO = CreateFilterVO(condition);
                    filterVOs.Add(itemVO);
                    lstFilters.SelectedItems.Add(itemVO);

                    conditions.Add(condition);
                }
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
            get => groupCondition;

            set
            {
                filterVOs.Clear();

                dataContextVO.PropertyChanged -= DataContextVO_PropertyChanged;
                groupCondition = value ?? new();
                dataContextVO.MatchMode = groupCondition.MatchMode;
                dataContextVO.IsInverted = groupCondition.IsInverted;
                dataContextVO.DefaultResult = groupCondition.DefaultResult;
                dataContextVO.IsDisabled = groupCondition.IsDisabled;

                groupCondition.Conditions ??= new();
                conditions = groupCondition.Conditions;
                foreach (Condition condition in conditions)
                {
                    filterVOs.Add(CreateFilterVO(condition));
                }

                dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            }
        }

        public ItemClipboard<Condition> ConditionClipboard
        {
            get => conditionClipboard;

            set
            {
                if (conditionClipboard == value)
                {
                    return;
                }

                if (conditionClipboard != null)
                {
                    conditionClipboard.ItemCopied -= ConditionClipboard_ItemCopied;
                    conditionClipboard.ItemPasted -= ConditionClipboard_ItemPasted;
                }

                conditionClipboard = value;

                if (conditionClipboard != null)
                {
                    conditionClipboard.ItemCopied += ConditionClipboard_ItemCopied;
                    conditionClipboard.ItemPasted += ConditionClipboard_ItemPasted;
                }
            }
        }
    }
}
