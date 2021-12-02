/* 2021/4/21 */
using Microsoft.Toolkit.Mvvm.Input;
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
        private readonly FilterListViewDataContextVO dataContextVO;

        private readonly ObservableCollection<FilterItemVO> filterVOs = new();

        private List<Condition> conditions;

        private GroupCondition groupCondition;

        private ItemClipboard<Condition> conditionListClipboard;

        private ItemClipboard<Condition> conditionTreeClipboard;

        public FilterListView()
        {
            InitializeComponent();

            conditions = new();
            groupCondition = new()
            {
                Id = Guid.NewGuid().ToString(),
                Conditions = conditions,
            };

            dataContextVO = new()
            {
                AddCommand = new RelayCommand(AddCondition, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedConditions, () => dataContextVO.IsAnyConditionSelected),
                EditCommand = new RelayCommand(EditSelectedConditions, () => dataContextVO.IsAnyConditionSelected),
                MoveLeftCommand = new RelayCommand(MoveSelectedConditionsLeft, () => dataContextVO.IsAnyConditionSelected),
                MoveRightCommand = new RelayCommand(MoveSelectedConditionsRight, () => dataContextVO.IsAnyConditionSelected),
                CutCommand = new RelayCommand(CutSelectedConditions,
                    () => dataContextVO.IsAnyConditionSelected && !dataContextVO.IsAnyConditionCutInClipboard),
                CopyCommand = new RelayCommand(CopySelectedConditions,
                    () => dataContextVO.IsAnyConditionSelected && !dataContextVO.IsAnyConditionCutInClipboard),
                PasteCommnad = new RelayCommand(PasteConditions, () => dataContextVO.IsAnyConditionInClipboard),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            lstFilters.ItemsSource = filterVOs;
        }

        private void DataContextVO_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FilterListViewDataContextVO.MatchMode):
                    groupCondition.MatchMode = dataContextVO.MatchMode;
                    break;
                case nameof(FilterListViewDataContextVO.IsInverted):
                    groupCondition.IsInverted = dataContextVO.IsInverted;
                    break;
                case nameof(FilterListViewDataContextVO.DefaultResult):
                    groupCondition.DefaultResult = dataContextVO.DefaultResult;
                    break;
                case nameof(FilterListViewDataContextVO.IsDisabled):
                    groupCondition.IsDisabled = dataContextVO.IsDisabled;
                    break;
                case nameof(FilterListViewDataContextVO.IsAnyConditionSelected):
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveLeftCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveRightCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FilterListViewDataContextVO.IsAnyConditionInClipboard):
                    (dataContextVO.PasteCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FilterListViewDataContextVO.IsAnyConditionCutInClipboard):
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstFilters.SelectedItems.Count;
            dataContextVO.IsAnyConditionSelected = count > 0;
        }

        private void LstFilters_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitResult.VisualHit.GetType() != typeof(ListBoxItem))
            {
                lstFilters.UnselectAll();
            }
        }

        private void LstFilters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelectedCondition();
        }

        private void ConditionClipboard_StatusChanged(object sender, EventArgs e)
        {
            UpdateClipboardStatus();
        }

        private void UpdateClipboardStatus()
        {
            dataContextVO.IsAnyConditionInClipboard = conditionListClipboard.ContainsItem;
            dataContextVO.IsAnyConditionCutInClipboard = conditionListClipboard.Status == ItemClipboardStatus.Move;
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

        private void InsertAtSelectedItem(params (Condition condition, FilterItemVO itemVO)[] conditionTuples)
        {
            var selectedIndex = lstFilters.SelectedIndex;
            if (selectedIndex == -1)
            {
                foreach (var (condition, itemVO) in conditionTuples)
                {
                    filterVOs.Add(itemVO);
                    lstFilters.SelectedItems.Add(itemVO);

                    conditions.Add(condition);
                }
            }
            else
            {
                lstFilters.SelectedItems.Clear();

                var reversedConditionTuples = conditionTuples.Reverse().ToArray();
                foreach (var (condition, itemVO) in reversedConditionTuples)
                {
                    filterVOs.Insert(selectedIndex, itemVO);
                    lstFilters.SelectedItems.Add(itemVO);

                    conditions.Insert(selectedIndex, condition);
                }
            }
        }

        private void AddCondition()
        {
            ConditionDetailWindow window = new()
            {
                ConditionClipboard = conditionTreeClipboard,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    var condition = window.Condition;

                    FilterItemVO itemVO = CreateFilterVO(condition);

                    InsertAtSelectedItem((condition, itemVO));
                }
            };

            window.Show();
        }

        private void RemoveSelectedConditions()
        {
            ForEachSelectedItem(RemoveCondition);
        }

        private void RemoveCondition(FilterItemVO itemVO)
        {
            var index = filterVOs.IndexOf(itemVO);
            filterVOs.RemoveAt(index);

            conditions.RemoveAt(index);
        }

        private void EditSelectedCondition()
        {
            ForSelectedItem(EditCondition);
        }

        private void EditSelectedConditions()
        {
            ForEachSelectedItem(EditCondition);
        }

        private void EditCondition(FilterItemVO itemVO)
        {
            var index = filterVOs.IndexOf(itemVO);

            var condition = conditions[index];
            ConditionDetailWindow window = new()
            {
                Condition = condition,
                ConditionClipboard = conditionTreeClipboard,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    var condition = window.Condition;

                    FilterItemVO item = CreateFilterVO(condition);

                    var newIndex = filterVOs.IndexOf(itemVO);
                    if (newIndex > -1)
                    {
                        filterVOs[newIndex] = item;
                        conditions[newIndex] = condition;
                    }
                }
            };

            window.Show();
        }

        private void MoveSelectedConditionsLeft()
        {
            ForEachSelectedItem(MoveConditionLeft, byOrder: true, recoverSelection: true);
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

        private void MoveSelectedConditionsRight()
        {
            ForEachSelectedItem(MoveConditionRight, byOrder: true, reverseOrder: true, recoverSelection: true);
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

        private void CutSelectedConditions()
        {
            if (conditionListClipboard != null)
            {
                List<Condition> selectedConditions = new();
                foreach (var selectedItem in lstFilters.SelectedItems)
                {
                    if (selectedItem is FilterItemVO itemVO)
                    {
                        var index = filterVOs.IndexOf(itemVO);

                        var condition = conditions[index];
                        selectedConditions.Add(condition);
                    }
                }

                conditionListClipboard.Cut(selectedConditions.ToArray());
                RemoveSelectedConditions();
            }
        }

        private void CopySelectedConditions()
        {
            if (conditionListClipboard != null)
            {
                List<Condition> copiedConditions = new();
                foreach (var selectedItem in lstFilters.SelectedItems)
                {
                    if (selectedItem is FilterItemVO itemVO)
                    {
                        var index = filterVOs.IndexOf(itemVO);

                        var condition = conditions[index];
                        copiedConditions.Add(condition);
                    }
                }

                conditionListClipboard.Copy(copiedConditions.ToArray());
            }
        }

        private void PasteConditions()
        {
            if (conditionListClipboard != null)
            {
                (var pastedConditions, var clipboardStatus) = conditionListClipboard.Paste();

                if (pastedConditions != null)
                {
                    var conditionTuples = pastedConditions.Select(pastedCondition =>
                    {
                        var condition = clipboardStatus == ItemClipboardStatus.Move ?
                            pastedCondition : (Condition)pastedCondition.Clone();

                        FilterItemVO itemVO = CreateFilterVO(condition);

                        return (condition, itemVO);
                    }).ToArray();

                    InsertAtSelectedItem(conditionTuples);
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

        public ItemClipboard<Condition> ConditionListClipboard
        {
            get => conditionListClipboard;

            set
            {
                if (conditionListClipboard == value)
                {
                    return;
                }

                if (conditionListClipboard != null)
                {
                    conditionListClipboard.ItemCut -= ConditionClipboard_StatusChanged;
                    conditionListClipboard.ItemCopied -= ConditionClipboard_StatusChanged;
                    conditionListClipboard.ItemPasted -= ConditionClipboard_StatusChanged;
                }

                conditionListClipboard = value;

                if (conditionListClipboard != null)
                {
                    conditionListClipboard.ItemCut += ConditionClipboard_StatusChanged;
                    conditionListClipboard.ItemCopied += ConditionClipboard_StatusChanged;
                    conditionListClipboard.ItemPasted += ConditionClipboard_StatusChanged;

                    UpdateClipboardStatus();
                }
            }
        }

        public ItemClipboard<Condition> ConditionTreeClipboard
        {
            get => conditionTreeClipboard;
            set => conditionTreeClipboard = value;
        }
    }
}
