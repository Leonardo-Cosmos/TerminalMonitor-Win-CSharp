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
        private readonly ObservableCollection<ConditionGroupNodeVO> rootConditions = new()
        {
            new ConditionGroupNodeVO()
        };

        public ConditionDetailWindow()
        {
            InitializeComponent();

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
                throw new ArgumentException("Unknown condition type");
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

            conditionGroup.Conditions
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
                throw new ArgumentException("Unknown condition VO type");
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
                    var conditionGroupVO =
                        rootConditions.Count > 0 ? rootConditions[0] : new ConditionGroupNodeVO();
                    return FromVO(conditionGroupVO);
                }
            }

            set
            {
                if (value == null)
                {
                    rdBtnSingle.IsChecked = true;
                    conditionView.FieldCondition = new FieldCondition();

                    rootConditions.Clear();
                    rootConditions.Add(new ConditionGroupNodeVO());

                    return;
                }

                if (value is FieldCondition fieldCondition)
                {
                    rdBtnSingle.IsChecked = true;
                    conditionView.FieldCondition = fieldCondition;

                    rootConditions.Clear();
                    rootConditions.Add(new ConditionGroupNodeVO());
                }
                else
                {
                    rdBtnMultiple.IsChecked = true;
                    var conditionGroupVO = ToVO(value) as ConditionGroupNodeVO;
                    rootConditions.Clear();
                    rootConditions.Add(conditionGroupVO);

                    conditionView.FieldCondition = new FieldCondition();
                }
            }
        }
    }
}
