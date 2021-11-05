/* 2021/7/9 */
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
using TerminalMonitor.Matchers.Models;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for ConditionDetailWindow.xaml
    /// </summary>
    public partial class ConditionDetailWindow : Window
    {
        private readonly ConditionDetailWindowDataContextVO dataContextVO = new();

        private readonly ObservableCollection<ConditionNodeVO> rootConditions = new();

        public ConditionDetailWindow()
        {
            InitializeComponent();

            DataContext = dataContextVO;

            rdBtnSingle.IsChecked = true;
            conditionView.FieldCondition = new FieldCondition();
            trConditions.ItemsSource = rootConditions;
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

        private void BtnAddField_Click(object sender, RoutedEventArgs e)
        {
            AddNodeVO(conditions => conditions.Add(new FieldConditionNodeVO() { Siblings = conditions }));
        }

        private void BtnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNodeVO(conditions => conditions.Add(new GroupConditionNodeVO() { Siblings = conditions }));
        }

        private void AddNodeVO(Action<ObservableCollection<ConditionNodeVO>> addNodeVO)
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem == null)
            {
                addNodeVO(rootConditions);
            }
            else if (selectedItem is GroupConditionNodeVO groupNodeVO)
            {
                addNodeVO(groupNodeVO.Conditions);
            }
            else if (selectedItem is FieldConditionNodeVO fieldNodeVO)
            {
                addNodeVO(fieldNodeVO.Siblings);
            }
            else
            {
                throw new NotImplementedException("Unknown condtion node type.");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem is ConditionNodeVO conditionNodeVO)
            {
                conditionNodeVO.Siblings.Remove(conditionNodeVO);
            }
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
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

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
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

            DialogResult = true;
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
                    .Select(conditionVO => FromVO(conditionVO)),

                IsInverted = groupConditionVO.IsInverted,
                DefaultResult = groupConditionVO.DefaultResult,
                IsDisabled = groupConditionVO.IsDisabled,
            };

            return groupCondition;
        }

        public Condition Condition
        {
            get
            {
                if (rdBtnSingle.IsChecked ?? false)
                {
                    return conditionView.FieldCondition;
                }
                else
                {
                    var groupCondition = new GroupCondition()
                    {
                        Name = dataContextVO.ConditionName,
                        IsInverted = dataContextVO.IsInverted,
                        DefaultResult = dataContextVO.DefaultResult,
                        IsDisabled = dataContextVO.IsDisabled,
                        MatchMode = dataContextVO.MatchMode,
                        Conditions = rootConditions
                            .Select(conditionNodeVO => FromVO(conditionNodeVO)).ToList(),
                    };

                    return groupCondition;
                }
            }

            set
            {
                if (value == null)
                {
                    rdBtnSingle.IsChecked = true;

                    /*
                     * Clear single condition.
                     */
                    conditionView.FieldCondition = new FieldCondition();

                    /*
                     * Clear multiple condtion.
                     */
                    rootConditions.Clear();

                    return;
                }

                if (value is FieldCondition fieldCondition)
                {
                    rdBtnSingle.IsChecked = true;
                    conditionView.FieldCondition = fieldCondition;

                    /*
                     * Clear multiple condtion.
                     */
                    rootConditions.Clear();
                }
                else if (value is GroupCondition groupCondition)
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
                    conditionView.FieldCondition = new FieldCondition();
                }
                else
                {
                    throw new NotImplementedException("Unknown condition type");
                }
            }
        }
    }
}
