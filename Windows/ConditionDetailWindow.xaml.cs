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

        private void MuItmAddConditionGroup_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var groupNodeVO = menuItem.Tag as ConditionGroupNodeVO;

            groupNodeVO.Conditions.Add(new ConditionGroupNodeVO());
        }

        private void MnItmAddCondition_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var groupNodeVO = menuItem.Tag as ConditionGroupNodeVO;

            groupNodeVO.Conditions.Add(new FieldConditionNodeVO());
        }

        private void BtnAddField_Click(object sender, RoutedEventArgs e)
        {
            AddNodeVO(conditions => conditions.Add(new FieldConditionNodeVO() { Siblings = conditions }));
        }

        private void BtnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNodeVO(conditions => conditions.Add(new ConditionGroupNodeVO() { Siblings = conditions }));
        }

        private void AddNodeVO(Action<ObservableCollection<ConditionNodeVO>> addNodeVO)
        {
            var selectedItem = trConditions.SelectedItem;

            if (selectedItem == null)
            {
                addNodeVO(rootConditions);
            }
            else if (selectedItem is ConditionGroupNodeVO groupNodeVO)
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

        private static ConditionNodeVO ToVO(Condition condition)
        {
            if (condition is FieldCondition fieldCondtion)
            {
                return ToVO(fieldCondtion);
            }
            else if (condition is ConditionGroup conditionGroup)
            {
                return ToVO(conditionGroup);
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

                NegativeMatch = fieldCondition.NegativeMatch,
                DefaultMatch = fieldCondition.DefaultMatch,
                DismissMatch = fieldCondition.DismissMatch,
            };

            return fieldConditionVO;
        }

        private static ConditionGroupNodeVO ToVO(ConditionGroup conditionGroup)
        {
            ConditionGroupNodeVO conditionGroupVO = new()
            {
                MatchMode = conditionGroup.MatchMode,

                NegativeMatch = conditionGroup.NegativeMatch,
                DefaultMatch = conditionGroup.DefaultMatch,
                DismissMatch = conditionGroup.DismissMatch,
            };

            conditionGroup.Conditions?
                .Select(condition => ToVO(condition))
                .ToList()
                .ForEach(conditionVO => conditionGroupVO.Conditions.Add(conditionVO));

            return conditionGroupVO;
        }

        private static Condition FromVO(ConditionNodeVO conditionVO)
        {
            if (conditionVO is FieldConditionNodeVO fieldCondtionVO)
            {
                return FromVO(fieldCondtionVO);
            }
            else if (conditionVO is ConditionGroupNodeVO conditionGroupVO)
            {
                return FromVO(conditionGroupVO);
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

                NegativeMatch = fieldConditionVO.NegativeMatch,
                DefaultMatch = fieldConditionVO.DefaultMatch,
                DismissMatch = fieldConditionVO.DismissMatch,
            };

            return fieldCondition;
        }

        private static ConditionGroup FromVO(ConditionGroupNodeVO conditionGroupVO)
        {
            ConditionGroup conditionGroup = new()
            {
                MatchMode = conditionGroupVO.MatchMode,
                Conditions = conditionGroupVO.Conditions
                    .Select(conditionVO => FromVO(conditionVO)),

                NegativeMatch = conditionGroupVO.NegativeMatch,
                DefaultMatch = conditionGroupVO.DefaultMatch,
                DismissMatch = conditionGroupVO.DismissMatch,
            };

            return conditionGroup;
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
                    var conditionGroup = new ConditionGroup()
                    {
                        Name = dataContextVO.ConditionName,
                        NegativeMatch = dataContextVO.NegativeMatch,
                        DefaultMatch = dataContextVO.DefaultMatch,
                        DismissMatch = dataContextVO.DismissMatch,
                        MatchMode = dataContextVO.MatchMode,
                        Conditions = rootConditions
                            .Select(conditionNodeVO => FromVO(conditionNodeVO)).ToList(),
                    };                    
                  
                    return conditionGroup;
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
                else if (value is ConditionGroup conditionGroup)
                {
                    rdBtnMultiple.IsChecked = true;
                    dataContextVO.ConditionName = conditionGroup.Name;
                    dataContextVO.NegativeMatch = conditionGroup.NegativeMatch;
                    dataContextVO.DefaultMatch = conditionGroup.DefaultMatch;
                    dataContextVO.DismissMatch = conditionGroup.DismissMatch;
                    dataContextVO.MatchMode = conditionGroup.MatchMode;
                    rootConditions.Clear();
                    conditionGroup.Conditions?
                        .Select(condition => ToVO(condition))
                        .ToList()
                        .ForEach(conditionNodeVO => rootConditions.Add(conditionNodeVO));

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
