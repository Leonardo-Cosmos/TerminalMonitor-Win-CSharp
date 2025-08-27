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
    /// Interaction logic for ConditionListView.xaml
    /// </summary>
    public partial class ConditionListView : UserControl
    {
        private readonly ConditionListViewDataContextVO dataContextVO;

        private readonly ObservableCollection<ConditionItemVO> conditionVOs = [];

        private List<Condition> conditions;

        private GroupCondition groupCondition;

        private ItemClipboard<Condition>? conditionListClipboard;

        private ItemClipboard<Condition>? conditionTreeClipboard;

        public ConditionListView()
        {
            InitializeComponent();

            conditions = [];
            groupCondition = new(GroupMatchMode.All, conditions);

            dataContextVO = new()
            {
                MatchMode = GroupMatchMode.All,

                AddCommand = new RelayCommand(AddCondition, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedConditions, () => dataContextVO!.IsAnyConditionSelected),
                EditCommand = new RelayCommand(EditSelectedConditions, () => dataContextVO!.IsAnyConditionSelected),
                ToggleInvertedCommnad = new RelayCommand(ToggleSelectedConditionsInverted, () => dataContextVO!.IsAnyConditionSelected),
                ToggleDefaultResultCommnad = new RelayCommand(ToggleSelectedConditionsDefaultResult, () => dataContextVO!.IsAnyConditionSelected),
                ToggleDisabledCommnad = new RelayCommand(ToggleSelectedConditionsDisabled, () => dataContextVO!.IsAnyConditionSelected),
                MoveLeftCommand = new RelayCommand(MoveSelectedConditionsLeft, () => dataContextVO!.IsAnyConditionSelected),
                MoveRightCommand = new RelayCommand(MoveSelectedConditionsRight, () => dataContextVO!.IsAnyConditionSelected),
                CutCommand = new RelayCommand(CutSelectedConditions,
                    () => dataContextVO!.IsAnyConditionSelected && !dataContextVO.IsAnyConditionCutInClipboard),
                CopyCommand = new RelayCommand(CopySelectedConditions,
                    () => dataContextVO!.IsAnyConditionSelected && !dataContextVO.IsAnyConditionCutInClipboard),
                PasteCommnad = new RelayCommand(PasteConditions, () => dataContextVO!.IsAnyConditionInClipboard),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            lstConditions.ItemsSource = conditionVOs;
        }

        private void DataContextVO_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConditionListViewDataContextVO.MatchMode):
                    groupCondition.MatchMode = dataContextVO.MatchMode;
                    break;
                case nameof(ConditionListViewDataContextVO.IsInverted):
                    groupCondition.IsInverted = dataContextVO.IsInverted;
                    break;
                case nameof(ConditionListViewDataContextVO.DefaultResult):
                    groupCondition.DefaultResult = dataContextVO.DefaultResult;
                    break;
                case nameof(ConditionListViewDataContextVO.IsDisabled):
                    groupCondition.IsDisabled = dataContextVO.IsDisabled;
                    break;
                case nameof(ConditionListViewDataContextVO.IsAnyConditionSelected):
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.ToggleInvertedCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.ToggleDefaultResultCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.ToggleDisabledCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveLeftCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveRightCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(ConditionListViewDataContextVO.IsAnyConditionInClipboard):
                    (dataContextVO.PasteCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(ConditionListViewDataContextVO.IsAnyConditionCutInClipboard):
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstConditions.SelectedItems.Count;
            dataContextVO.IsAnyConditionSelected = count > 0;
        }

        private void LstConditions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitResult.VisualHit.GetType() != typeof(ListBoxItem))
            {
                lstConditions.UnselectAll();
            }
        }

        private void LstConditions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelectedCondition();
        }

        private void ConditionClipboard_StatusChanged(object? sender, EventArgs e)
        {
            UpdateClipboardStatus();
        }

        private void UpdateClipboardStatus()
        {
            if (conditionListClipboard == null)
            {
                return;
            }
            dataContextVO.IsAnyConditionInClipboard = conditionListClipboard.ContainsItem;
            dataContextVO.IsAnyConditionCutInClipboard = conditionListClipboard.Status == ItemClipboardStatus.Move;
        }

        private void ForSelectedItem(Action<ConditionItemVO> action)
        {
            if (lstConditions.SelectedItem is ConditionItemVO itemVO)
            {
                action(itemVO);
            }
        }

        private void ForEachSelectedItem(Action<ConditionItemVO> action,
            bool byOrder = false, bool reverseOrder = false, bool recoverSelection = false)
        {
            List<ConditionItemVO> itemVOs = [];
            foreach (var selectedItem in lstConditions.SelectedItems)
            {
                if (selectedItem is ConditionItemVO itemVO)
                {
                    itemVOs.Add(itemVO);
                }
            }

            if (byOrder)
            {
                itemVOs.Sort((itemX, itemY) =>
                    conditionVOs.IndexOf(itemX) - conditionVOs.IndexOf(itemY));
            }

            if (reverseOrder)
            {
                itemVOs.Reverse();
            }

            itemVOs.ForEach(action);

            if (recoverSelection)
            {
                itemVOs.ForEach(itemVO => lstConditions.SelectedItems.Add(itemVO));
            }
        }

        private void InsertAtSelectedItem(params (Condition condition, ConditionItemVO itemVO)[] conditionTuples)
        {
            var selectedIndex = lstConditions.SelectedIndex;
            if (selectedIndex == -1)
            {
                foreach (var (condition, itemVO) in conditionTuples)
                {
                    conditionVOs.Add(itemVO);
                    lstConditions.SelectedItems.Add(itemVO);

                    conditions.Add(condition);
                }
            }
            else
            {
                lstConditions.SelectedItems.Clear();

                var reversedConditionTuples = conditionTuples.Reverse().ToArray();
                foreach (var (condition, itemVO) in reversedConditionTuples)
                {
                    conditionVOs.Insert(selectedIndex, itemVO);
                    lstConditions.SelectedItems.Add(itemVO);

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

            window.Closing += (object? sender, CancelEventArgs e) =>
            {
                if (window.IsSaved && window.Condition != null)
                {
                    var condition = window.Condition;

                    ConditionItemVO itemVO = CreateConditionVO(condition);

                    InsertAtSelectedItem((condition, itemVO));
                }
            };

            window.Show();
        }

        private void RemoveSelectedConditions()
        {
            ForEachSelectedItem(RemoveCondition);
        }

        private void RemoveCondition(ConditionItemVO itemVO)
        {
            var index = conditionVOs.IndexOf(itemVO);
            conditionVOs.RemoveAt(index);

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

        private void EditCondition(ConditionItemVO itemVO)
        {
            var index = conditionVOs.IndexOf(itemVO);

            var condition = conditions[index];
            ConditionDetailWindow window = new()
            {
                Condition = condition,
                ConditionClipboard = conditionTreeClipboard,
            };

            window.Closing += (object? sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    var condition = window.Condition;

                    ConditionItemVO item = CreateConditionVO(condition);

                    var newIndex = conditionVOs.IndexOf(itemVO);
                    if (newIndex > -1)
                    {
                        conditionVOs[newIndex] = item;
                        conditions[newIndex] = condition;
                    }
                }
            };

            window.Show();
        }

        private void ToggleSelectedConditionsInverted()
        {
            ForEachSelectedItem(ToggleConditionInverted);
        }

        private void ToggleConditionInverted(ConditionItemVO itemVO)
        {
            var index = conditionVOs.IndexOf(itemVO);
            var condition = conditions[index];

            condition.IsInverted = !condition.IsInverted;
            itemVO.IsInverted = condition.IsInverted;
        }

        private void ToggleSelectedConditionsDefaultResult()
        {
            ForEachSelectedItem(ToggleConditionDefaultResult);
        }

        private void ToggleConditionDefaultResult(ConditionItemVO itemVO)
        {
            var index = conditionVOs.IndexOf(itemVO);
            var condition = conditions[index];

            condition.DefaultResult = !condition.DefaultResult;
            itemVO.DefaultResult = condition.DefaultResult;
        }

        private void ToggleSelectedConditionsDisabled()
        {
            ForEachSelectedItem(ToggleConditionDisabled);
        }

        private void ToggleConditionDisabled(ConditionItemVO itemVO)
        {
            var index = conditionVOs.IndexOf(itemVO);
            var condition = conditions[index];

            condition.IsDisabled = !condition.IsDisabled;
            itemVO.IsDisabled = condition.IsDisabled;
        }

        private void MoveSelectedConditionsLeft()
        {
            ForEachSelectedItem(MoveConditionLeft, byOrder: true, recoverSelection: true);
        }

        private void MoveConditionLeft(ConditionItemVO itemVO)
        {
            var srcIndex = conditionVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex - 1 + conditionVOs.Count) % conditionVOs.Count;

            conditionVOs.RemoveAt(srcIndex);
            conditionVOs.Insert(dstIndex, itemVO);

            var condition = conditions[srcIndex];
            conditions.RemoveAt(srcIndex);
            conditions.Insert(dstIndex, condition);
        }

        private void MoveSelectedConditionsRight()
        {
            ForEachSelectedItem(MoveConditionRight, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void MoveConditionRight(ConditionItemVO itemVO)
        {
            var srcIndex = conditionVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex + 1) % conditionVOs.Count;

            conditionVOs.RemoveAt(srcIndex);
            conditionVOs.Insert(dstIndex, itemVO);

            var condition = conditions[srcIndex];
            conditions.RemoveAt(srcIndex);
            conditions.Insert(dstIndex, condition);
        }

        private void CutSelectedConditions()
        {
            if (conditionListClipboard != null)
            {
                List<Condition> selectedConditions = [];
                foreach (var selectedItem in lstConditions.SelectedItems)
                {
                    if (selectedItem is ConditionItemVO itemVO)
                    {
                        var index = conditionVOs.IndexOf(itemVO);

                        var condition = conditions[index];
                        selectedConditions.Add(condition);
                    }
                }

                conditionListClipboard.Cut([.. selectedConditions]);
                RemoveSelectedConditions();
            }
        }

        private void CopySelectedConditions()
        {
            if (conditionListClipboard != null)
            {
                List<Condition> copiedConditions = [];
                foreach (var selectedItem in lstConditions.SelectedItems)
                {
                    if (selectedItem is ConditionItemVO itemVO)
                    {
                        var index = conditionVOs.IndexOf(itemVO);

                        var condition = conditions[index];
                        copiedConditions.Add(condition);
                    }
                }

                conditionListClipboard.Copy([.. copiedConditions]);
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

                        ConditionItemVO itemVO = CreateConditionVO(condition);

                        return (condition, itemVO);
                    }).ToArray();

                    InsertAtSelectedItem(conditionTuples);
                }
            }
        }

        private static ConditionItemVO CreateConditionVO(Condition condition)
        {
            ConditionItemVO item;
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

        public void AddCondition(Condition condition)
        {
            if (condition == null)
            {
                return;
            }

            ConditionItemVO itemVO = CreateConditionVO(condition);

            InsertAtSelectedItem((condition, itemVO));
        }

        public GroupCondition Condition
        {
            get => groupCondition;

            set
            {
                conditionVOs.Clear();

                dataContextVO.PropertyChanged -= DataContextVO_PropertyChanged;
                groupCondition = value ?? GroupCondition.Empty;
                dataContextVO.MatchMode = groupCondition.MatchMode;
                dataContextVO.IsInverted = groupCondition.IsInverted;
                dataContextVO.DefaultResult = groupCondition.DefaultResult;
                dataContextVO.IsDisabled = groupCondition.IsDisabled;

                groupCondition.Conditions ??= [];
                conditions = groupCondition.Conditions;
                foreach (Condition condition in conditions)
                {
                    conditionVOs.Add(CreateConditionVO(condition));
                }

                dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            }
        }

        public ItemClipboard<Condition>? ConditionListClipboard
        {
            get => conditionListClipboard;

            set
            {
                if (Object.ReferenceEquals(conditionListClipboard, value))
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

        public ItemClipboard<Condition>? ConditionTreeClipboard
        {
            get => conditionTreeClipboard;
            set => conditionTreeClipboard = value;
        }
    }
}
