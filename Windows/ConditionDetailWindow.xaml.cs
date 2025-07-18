﻿/* 2021/7/9 */
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TerminalMonitor.Clipboard;
using TerminalMonitor.Matchers.Models;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for ConditionDetailWindow.xaml
    /// </summary>
    public partial class ConditionDetailWindow : Window
    {
        private readonly ConditionDetailWindowDataContextVO dataContextVO;

        private readonly ObservableCollection<ConditionNodeVO> rootConditions = [];

        private Condition? condition;

        private ItemClipboard<Condition>? conditionClipboard;

        public ConditionDetailWindow()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                AddFieldCommand = new RelayCommand(AddFieldCondition, () => true),
                AddGroupCommand = new RelayCommand(AddGroupCondition, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedCondition, () => dataContextVO!.IsConditionSelected),
                MoveUpCommand = new RelayCommand(MoveSelectedConditionUp, () => dataContextVO!.IsConditionSelected),
                MoveDownCommand = new RelayCommand(MoveSelectedConditionDown, () => dataContextVO!.IsConditionSelected),
                CutCommand = new RelayCommand(CutSelectedCondition,
                    () => dataContextVO!.IsConditionSelected && !dataContextVO.IsConditionCutInClipboard),
                CopyCommand = new RelayCommand(CopySelectedCondition,
                    () => dataContextVO!.IsConditionSelected && !dataContextVO.IsConditionCutInClipboard),
                PasteCommnad = new RelayCommand(PasteCondition, () => dataContextVO!.IsConditionInClipboard),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            rdBtnSingle.IsChecked = true;
            fieldConditionView.FieldCondition = FieldCondition.Empty;
            trConditions.ItemsSource = rootConditions;
        }

        private void DataContextVO_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConditionDetailWindowDataContextVO.IsConditionSelected):
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveUpCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveDownCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(ConditionDetailWindowDataContextVO.IsConditionInClipboard):
                    (dataContextVO.PasteCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(ConditionDetailWindowDataContextVO.IsConditionCutInClipboard):
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void RdBtnCondition_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == rdBtnSingle)
            {
                grpBxSingleField.IsEnabled = true;
                grpBxMultipleField.IsEnabled = false;
            }
            else
            {
                grpBxSingleField.IsEnabled = false;
                grpBxMultipleField.IsEnabled = true;
            }
        }

        private void TrConditions_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dataContextVO.IsConditionSelected = trConditions.SelectedItem != null;
        }

        private void TrConditions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitResult.VisualHit.GetType() != typeof(ConditionNodeVO) &&
                e.ChangedButton == MouseButton.Left &&
                trConditions.SelectedItem is ConditionNodeVO conditionNodeVO)
            {
                UnselectConditionTreeNode(conditionNodeVO);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (rdBtnMultiple.IsChecked ?? false)
            {
                txtBxConditionName.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                if (Validation.GetHasError(txtBxConditionName))
                {
                    txtBxConditionName.Focus();
                    return;
                }
            }

            SaveCondition();
            IsSaved = true;
            Close();
        }

        private void ConditionClipboard_StatusChanged(object? sender, EventArgs e)
        {
            UpdateClipboardStatus();
        }

        private void UpdateClipboardStatus()
        {
            if (conditionClipboard != null)
            {
                dataContextVO.IsConditionInClipboard = conditionClipboard.ContainsItem;
                dataContextVO.IsConditionCutInClipboard = conditionClipboard.Status == ItemClipboardStatus.Move;
            }
        }

        private void AddFieldCondition()
        {
            AddCondition(conditions => new FieldConditionNodeVO()
            {
                FieldKey = String.Empty,
                MatchOperator = Matchers.TextMatchOperator.None,
                TargetValue = String.Empty,

                Siblings = conditions,
            });
        }

        private void AddGroupCondition()
        {
            AddCondition(conditions => new GroupConditionNodeVO()
            {
                MatchMode = GroupMatchMode.All,

                Siblings = conditions,
            });
        }

        private void AddCondition(Func<ObservableCollection<ConditionNodeVO>, ConditionNodeVO> createNodeVO)
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem == null)
            {
                var conditionNodeVO = createNodeVO(rootConditions);
                rootConditions.Add(conditionNodeVO);
            }
            else if (selectedItem is GroupConditionNodeVO groupNodeVO)
            {
                var conditionNodeVO = createNodeVO(groupNodeVO.Conditions);
                groupNodeVO.Conditions.Add(conditionNodeVO);
            }
            else if (selectedItem is FieldConditionNodeVO fieldNodeVO)
            {
                var conditionNodeVO = createNodeVO(fieldNodeVO.Siblings);
                var index = fieldNodeVO.Siblings.IndexOf(fieldNodeVO);
                fieldNodeVO.Siblings.Insert(index, conditionNodeVO);
            }
            else
            {
                throw new NotImplementedException("Unknown condtion node type.");
            }
        }

        private void RemoveSelectedCondition()
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem is ConditionNodeVO conditionNodeVO)
            {
                conditionNodeVO.Siblings.Remove(conditionNodeVO);
            }
        }

        private void MoveSelectedConditionUp()
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem is ConditionNodeVO conditionNodeVO)
            {
                var siblings = conditionNodeVO.Siblings;
                var index = siblings.IndexOf(conditionNodeVO);
                if (index > 0)
                {
                    siblings.Remove(conditionNodeVO);
                    siblings.Insert(index - 1, conditionNodeVO);

                    SelectConditionTreeNode(conditionNodeVO);
                }
            }
        }

        private void MoveSelectedConditionDown()
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem is ConditionNodeVO conditionNodeVO)
            {
                var siblings = conditionNodeVO.Siblings;
                var index = siblings.IndexOf(conditionNodeVO);
                if (index < siblings.Count - 1)
                {
                    siblings.Remove(conditionNodeVO);
                    siblings.Insert(index + 1, conditionNodeVO);

                    SelectConditionTreeNode(conditionNodeVO);
                }
            }
        }

        private void CutSelectedCondition()
        {
            if (conditionClipboard != null)
            {
                var selectedItem = trConditions.SelectedItem;
                if (selectedItem is ConditionNodeVO conditionNodeVO)
                {
                    var selectedCondition = FromVO(conditionNodeVO);

                    conditionClipboard.Cut(selectedCondition);
                }

                RemoveSelectedCondition();
            }
        }

        private void CopySelectedCondition()
        {
            if (conditionClipboard != null)
            {
                var selectedItem = trConditions.SelectedItem;
                if (selectedItem is ConditionNodeVO conditionNodeVO)
                {
                    var selectedCondition = FromVO(conditionNodeVO);

                    conditionClipboard.Copy(selectedCondition);
                }
            }
        }

        private void PasteCondition()
        {
            if (conditionClipboard != null)
            {
                (var pastedConditions, var clipboardStatus) = conditionClipboard.Paste();

                if (pastedConditions?.Length > 0)
                {
                    var addedConditions = clipboardStatus == ItemClipboardStatus.Move ?
                        pastedConditions :
                        [.. pastedConditions.Select(condition => (Condition)condition.Clone())];

                    foreach (var addedCondition in addedConditions)
                    {
                        var conditionVO = ToVO(addedCondition);
                        AddCondition(conditions =>
                        {
                            conditionVO.Siblings = conditions;
                            return conditionVO;
                        });
                    }
                }
            }
        }

        private void SelectConditionTreeNode(ConditionNodeVO conditionVO)
        {
            if (trConditions.ItemContainerGenerator.ContainerFromItem(conditionVO) is TreeViewItem treeViewItem)
            {
                treeViewItem.IsSelected = true;
            }
        }

        private void UnselectConditionTreeNode(ConditionNodeVO conditionVO)
        {
            if (trConditions.ItemContainerGenerator.ContainerFromItem(conditionVO) is TreeViewItem treeViewItem)
            {
                treeViewItem.IsSelected = false;
            }
        }

        private static ConditionNodeVO ToVO(Condition condition)
        {
            if (condition is FieldCondition fieldCondtion)
            {
                return ToVO(fieldCondtion);
            }
            else if (condition is GroupCondition groupCondition)
            {
                return ToVO(groupCondition);
            }
            else
            {
                throw new NotImplementedException("Unknown condition type");
            }
        }
        private static FieldConditionNodeVO ToVO(FieldCondition fieldCondition)
        {
            FieldConditionNodeVO fieldConditionVO = new()
            {
                FieldKey = fieldCondition.FieldKey,
                MatchOperator = fieldCondition.MatchOperator,
                TargetValue = fieldCondition.TargetValue,

                IsInverted = fieldCondition.IsInverted,
                DefaultResult = fieldCondition.DefaultResult,
                IsDisabled = fieldCondition.IsDisabled,

                Siblings = [],
            };

            return fieldConditionVO;
        }

        private static GroupConditionNodeVO ToVO(GroupCondition groupCondition)
        {
            GroupConditionNodeVO groupConditionVO = new()
            {
                MatchMode = groupCondition.MatchMode,

                IsInverted = groupCondition.IsInverted,
                DefaultResult = groupCondition.DefaultResult,
                IsDisabled = groupCondition.IsDisabled,

                Siblings = [],
            };

            groupCondition.Conditions?
                .Select(condition => ToVO(condition))
                .ToList()
                .ForEach(conditionVO =>
                {
                    conditionVO.Siblings = groupConditionVO.Conditions;
                    groupConditionVO.Conditions.Add(conditionVO);
                });

            return groupConditionVO;
        }

        private static Condition FromVO(ConditionNodeVO conditionVO)
        {
            if (conditionVO is FieldConditionNodeVO fieldCondtionVO)
            {
                return FromVO(fieldCondtionVO);
            }
            else if (conditionVO is GroupConditionNodeVO groupConditionVO)
            {
                return FromVO(groupConditionVO);
            }
            else
            {
                throw new NotImplementedException("Unknown condition VO type");
            }
        }

        private static FieldCondition FromVO(FieldConditionNodeVO fieldConditionVO)
        {
            FieldCondition fieldCondition = new(
                fieldConditionVO.FieldKey,
                fieldConditionVO.MatchOperator,
                fieldConditionVO.TargetValue)
            {
                IsInverted = fieldConditionVO.IsInverted,
                DefaultResult = fieldConditionVO.DefaultResult,
                IsDisabled = fieldConditionVO.IsDisabled,
            };

            return fieldCondition;
        }

        private static GroupCondition FromVO(GroupConditionNodeVO groupConditionVO)
        {
            GroupCondition groupCondition = new(
                groupConditionVO.MatchMode,
                [.. groupConditionVO.Conditions.Select(conditionVO => FromVO(conditionVO))])
            {
                IsInverted = groupConditionVO.IsInverted,
                DefaultResult = groupConditionVO.DefaultResult,
                IsDisabled = groupConditionVO.IsDisabled,
            };

            return groupCondition;
        }

        private void LoadCondition(Condition? condition)
        {
            this.condition = condition;
            IsSaved = false;

            if (condition == null)
            {
                rdBtnSingle.IsChecked = true;

                /*
                 * Clear single condition.
                 */
                fieldConditionView.FieldCondition = FieldCondition.Empty;

                /*
                 * Clear multiple condtion.
                 */
                rootConditions.Clear();

                return;
            }

            if (condition is FieldCondition fieldCondition)
            {
                rdBtnSingle.IsChecked = true;
                fieldConditionView.FieldCondition = fieldCondition;

                /*
                 * Clear multiple condtion.
                 */
                rootConditions.Clear();
            }
            else if (condition is GroupCondition groupCondition)
            {
                rdBtnMultiple.IsChecked = true;
                dataContextVO.ConditionName = groupCondition.Name;
                dataContextVO.IsInverted = groupCondition.IsInverted;
                dataContextVO.DefaultResult = groupCondition.DefaultResult;
                dataContextVO.IsDisabled = groupCondition.IsDisabled;
                dataContextVO.MatchMode = groupCondition.MatchMode;
                rootConditions.Clear();
                groupCondition.Conditions?
                    .Select(condition => ToVO(condition))
                    .ToList()
                    .ForEach(conditionNodeVO =>
                    {
                        conditionNodeVO.Siblings = rootConditions;
                        rootConditions.Add(conditionNodeVO);
                    });

                /*
                 * Clear single condition.
                 */
                fieldConditionView.FieldCondition = FieldCondition.Empty;
            }
            else
            {
                throw new NotImplementedException("Unknown condition type");
            }
        }

        private void SaveCondition()
        {
            if (rdBtnSingle.IsChecked ?? false)
            {
                condition = fieldConditionView.FieldCondition;
            }
            else
            {
                if (condition is GroupCondition groupCondition)
                {
                    groupCondition.Name = dataContextVO.ConditionName;
                    groupCondition.IsInverted = dataContextVO.IsInverted;
                    groupCondition.DefaultResult = dataContextVO.DefaultResult;
                    groupCondition.IsDisabled = dataContextVO.IsDisabled;
                    groupCondition.MatchMode = dataContextVO.MatchMode;
                    groupCondition.Conditions = [.. rootConditions.Select(conditionNodeVO => FromVO(conditionNodeVO))];
                }
                else
                {
                    condition = new GroupCondition(
                        dataContextVO.ConditionName,
                        dataContextVO.MatchMode,
                        [.. rootConditions.Select(conditionNodeVO => FromVO(conditionNodeVO))])
                    {

                        IsInverted = dataContextVO.IsInverted,
                        DefaultResult = dataContextVO.DefaultResult,
                        IsDisabled = dataContextVO.IsDisabled,

                    };
                }
            }
        }

        public bool IsSaved { get; set; }

        public Condition? Condition
        {
            get => condition;
            set => LoadCondition(value);
        }

        public ItemClipboard<Condition>? ConditionClipboard
        {
            get => conditionClipboard;

            set
            {
                if (Object.ReferenceEquals(conditionClipboard, value))
                {
                    return;
                }

                if (conditionClipboard != null)
                {
                    conditionClipboard.ItemCut -= ConditionClipboard_StatusChanged;
                    conditionClipboard.ItemCopied -= ConditionClipboard_StatusChanged;
                    conditionClipboard.ItemPasted -= ConditionClipboard_StatusChanged;
                }

                conditionClipboard = value;

                if (conditionClipboard != null)
                {
                    conditionClipboard.ItemCut += ConditionClipboard_StatusChanged;
                    conditionClipboard.ItemCopied += ConditionClipboard_StatusChanged;
                    conditionClipboard.ItemPasted += ConditionClipboard_StatusChanged;

                    UpdateClipboardStatus();
                }
            }
        }
    }
}
