/* 2021/7/9 */
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly ObservableCollection<ConditionNodeVO> rootConditions = new();

        private Condition condition;

        private ItemClipboard<Condition> conditionClipboard;

        public ConditionDetailWindow()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                AddFieldCommand = new RelayCommand(AddFieldCondition, () => true),
                AddGroupCommand = new RelayCommand(AddGroupCondition, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedCondition, () => dataContextVO.IsConditionSelected),
                MoveUpCommand = new RelayCommand(MoveSelectedConditionUp, () => dataContextVO.IsConditionSelected),
                MoveDownCommand = new RelayCommand(MoveSelectedConditionDown, () => dataContextVO.IsConditionSelected),
                CutCommand = new RelayCommand(CutSelectedCondition, () => dataContextVO.IsConditionSelected),
                CopyCommand = new RelayCommand(CopySelectedCondition, () => dataContextVO.IsConditionSelected),
                PasteCommnad = new RelayCommand(PasteCondition, () => dataContextVO.IsConditionInClipboard),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            rdBtnSingle.IsChecked = true;
            fieldConditionView.FieldCondition = new FieldCondition();
            trConditions.ItemsSource = rootConditions;
        }

        private void DataContextVO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void ConditionClipboard_ItemCut(object sender, EventArgs e)
        {
            dataContextVO.IsConditionInClipboard = conditionClipboard?.ContainsItem ?? false;
        }

        private void ConditionClipboard_ItemCopied(object sender, EventArgs e)
        {
            dataContextVO.IsConditionInClipboard = conditionClipboard?.ContainsItem ?? false;
        }

        private void ConditionClipboard_ItemPasted(object sender, EventArgs e)
        {
            dataContextVO.IsConditionInClipboard = conditionClipboard?.ContainsItem ?? false;
        }

        private void AddFieldCondition()
        {
            AddCondition(conditions => new FieldConditionNodeVO() { Siblings = conditions });
        }

        private void AddGroupCondition()
        {
            AddCondition(conditions => new GroupConditionNodeVO() { Siblings = conditions });
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
            var selectedItem = trConditions.SelectedItem;
            if (selectedItem is ConditionNodeVO conditionNodeVO)
            {
                var selectedCondition = FromVO(conditionNodeVO);

                conditionClipboard?.Cut(selectedCondition);
            }

            RemoveSelectedCondition();
        }

        private void CopySelectedCondition()
        {
            var selectedItem = trConditions.SelectedItem;
            if (selectedItem is ConditionNodeVO conditionNodeVO)
            {
                var selectedCondition = FromVO(conditionNodeVO);

                conditionClipboard?.Copy(selectedCondition);
            }
        }

        private void PasteCondition()
        {
            if (conditionClipboard != null)
            {
                (var pastedConditions, var status) = conditionClipboard.Paste();

                if (pastedConditions != null && pastedConditions.Length > 0)
                {
                    Condition pastedCondition;
                    if (status == ItemClipboardStatus.Move)
                    {
                        pastedCondition = pastedConditions[0];
                    }
                    else
                    {
                        pastedCondition = (Condition)pastedConditions[0].Clone();
                    }

                    var conditionVO = ToVO(pastedCondition);
                    AddCondition(conditions =>
                    {
                        conditionVO.Siblings = conditions;
                        return conditionVO;
                    });
                }
            }
        }

        private void SelectConditionTreeNode(ConditionNodeVO conditionVO)
        {
            var treeViewItem =
                trConditions.ItemContainerGenerator.ContainerFromItem(conditionVO) as TreeViewItem;

            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
            }
        }

        private void UnselectConditionTreeNode(ConditionNodeVO conditionVO)
        {
            var treeViewItem =
                trConditions.ItemContainerGenerator.ContainerFromItem(conditionVO) as TreeViewItem;

            if (treeViewItem != null)
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
            FieldCondition fieldCondition = new()
            {
                FieldKey = fieldConditionVO.FieldKey,
                MatchOperator = fieldConditionVO.MatchOperator,
                TargetValue = fieldConditionVO.TargetValue,

                IsInverted = fieldConditionVO.IsInverted,
                DefaultResult = fieldConditionVO.DefaultResult,
                IsDisabled = fieldConditionVO.IsDisabled,
            };

            return fieldCondition;
        }

        private static GroupCondition FromVO(GroupConditionNodeVO groupConditionVO)
        {
            GroupCondition groupCondition = new()
            {
                MatchMode = groupConditionVO.MatchMode,
                Conditions = groupConditionVO.Conditions
                    .Select(conditionVO => FromVO(conditionVO)).ToList(),

                IsInverted = groupConditionVO.IsInverted,
                DefaultResult = groupConditionVO.DefaultResult,
                IsDisabled = groupConditionVO.IsDisabled,
            };

            return groupCondition;
        }

        private void LoadCondition(Condition condition)
        {
            this.condition = condition;
            IsSaved = false;

            if (condition == null)
            {
                rdBtnSingle.IsChecked = true;

                /*
                 * Clear single condition.
                 */
                fieldConditionView.FieldCondition = new FieldCondition();

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
                fieldConditionView.FieldCondition = new FieldCondition();
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
                    groupCondition.Conditions = rootConditions
                        .Select(conditionNodeVO => FromVO(conditionNodeVO)).ToList();
                }
                else
                {
                    condition = new GroupCondition()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = dataContextVO.ConditionName,
                        IsInverted = dataContextVO.IsInverted,
                        DefaultResult = dataContextVO.DefaultResult,
                        IsDisabled = dataContextVO.IsDisabled,
                        MatchMode = dataContextVO.MatchMode,
                        Conditions = rootConditions
                            .Select(conditionNodeVO => FromVO(conditionNodeVO)).ToList(),
                    };
                }
            }
        }

        public bool IsSaved { get; set; }

        public Condition Condition
        {
            get => condition;
            set => LoadCondition(value);
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
                    conditionClipboard.ItemCut -= ConditionClipboard_ItemCut;
                    conditionClipboard.ItemCopied -= ConditionClipboard_ItemCopied;
                    conditionClipboard.ItemPasted -= ConditionClipboard_ItemPasted;
                }

                conditionClipboard = value;

                if (conditionClipboard != null)
                {
                    conditionClipboard.ItemCut += ConditionClipboard_ItemCut;
                    conditionClipboard.ItemCopied += ConditionClipboard_ItemCopied;
                    conditionClipboard.ItemPasted += ConditionClipboard_ItemPasted;
                }
            }
        }
    }
}
