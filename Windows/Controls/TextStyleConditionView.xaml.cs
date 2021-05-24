/* 2021/5/23 */
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TextStyleCondtionView.xaml
    /// </summary>
    public partial class TextStyleConditionView : UserControl
    {
        private readonly TextStyleConditionViewDataContextVO dataContextVO = new()
        {
            Style = new TextStyle(),
            Conditions = new ObservableCollection<TextStyleCondition>(),
        };

        public TextStyleConditionView()
        {
            InitializeComponent();

            DataContext = dataContextVO;
            lstStyleCondtions.ItemsSource = dataContextVO.Conditions;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            TextStyleCondition textStyleCondition = new()
            {
                Style = new TextStyle()
                {

                },
                Condition = new TextCondition()
                {
                    FieldKey = "time",
                    MatchOperator = Matchers.TextMatcher.MatchOperator.None,
                    TargetValue = "",
                }
            };
            dataContextVO.Conditions.Add(textStyleCondition);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstStyleCondtions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
