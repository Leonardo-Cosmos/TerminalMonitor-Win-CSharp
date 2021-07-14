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

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for ConditionDetailWindow.xaml
    /// </summary>
    public partial class ConditionDetailWindow : Window
    {
        private readonly Array matchModes = Enum.GetValues(typeof(GroupMatchMode));

        internal Array MatchModes => matchModes;

        public ConditionDetailWindow()
        {
            InitializeComponent();

            trConditions.ItemsSource = new List<ConditionGroupNodeVO>()
            {
                new ConditionGroupNodeVO() {
                }
            };
        }

        private void BtnAddConditionGroup_Click(object sender, RoutedEventArgs e)
        {
            var button = e.Source as Button;
            var groupNodeVO = button.Tag as ConditionGroupNodeVO;

            groupNodeVO.Conditions.Add(new ConditionGroupNodeVO());
        }

        private void BtnAddCondition_Click(object sender, RoutedEventArgs e)
        {
            var button = e.Source as Button;
            var groupNodeVO = button.Tag as ConditionGroupNodeVO;

            groupNodeVO.Conditions.Add(new FieldConditionNodeVO());
        }
    }
}
