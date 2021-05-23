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
            DefaultStyle = new TextStyle(),
            Conditions = new ObservableCollection<TextStyleCondition>(),
        };

        public TextStyleConditionView()
        {
            InitializeComponent();

            DataContext = dataContextVO;
            lstStyleCondtions.ItemsSource = dataContextVO.Conditions;
        }
    }
}
