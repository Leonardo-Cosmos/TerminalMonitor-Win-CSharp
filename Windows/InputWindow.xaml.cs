/* 2021/6/25 */
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
using System.Windows.Shapes;
namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        private readonly InputWindowDataContext dataContextVO = new()
        {
            Message = String.Empty,
            Text = String.Empty,
        };

        public InputWindow()
        {
            InitializeComponent();
            DataContext = dataContextVO;
        }

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Message
        {
            get => dataContextVO.Message;
            set => dataContextVO.Message = value;
        }

        public string Text
        {
            get => dataContextVO.Text;
            set
            {
                dataContextVO.Text = value;

                txtText.SelectAll();
                txtText.Focus();
            }
        }
    }
}
