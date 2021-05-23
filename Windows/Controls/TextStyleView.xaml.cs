/* 2021/5/23 */
using System;
using System.Collections.Generic;
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
    /// Interaction logic for TextStyleView.xaml
    /// </summary>
    public partial class TextStyleView : UserControl
    {
        private readonly TextStyleViewDataContextVO dataContextVO = new()
        {
            Foreground = Brushes.Black,
            Background = Brushes.White,
        };

        public TextStyleView()
        {
            InitializeComponent();

            DataContext = dataContextVO;
        }

        public TextStyle StyleDetail
        {
            get
            {
                return new TextStyle()
                {
                    Foreground = (dataContextVO.Foreground as SolidColorBrush).Color,
                    Background = (dataContextVO.Background as SolidColorBrush).Color,
                };
            }

            set
            {
                if (value != null)
                {
                    dataContextVO.Foreground = new SolidColorBrush(value.Foreground);
                    dataContextVO.Background = new SolidColorBrush(value.Background);
                }
            }
        }
    }
}
