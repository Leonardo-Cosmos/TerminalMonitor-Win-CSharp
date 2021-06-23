/* 2021/6/23 */
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
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for TerminalLineDetailWindow.xaml
    /// </summary>
    public partial class TerminalLineDetailWindow : Window
    {
        private TerminalLineDto terminalLineDto;

        public TerminalLineDetailWindow()
        {
            InitializeComponent();
        }

        private void SetTerminalLineFields(Dictionary<string, TerminalLineFieldDto> lineFieldsDict)
        {
            var fields = lineFieldsDict.OrderBy(kvPair => kvPair.Key)
                .Select(kvPair => kvPair.Value)
                .ToList();
            lstFields.ItemsSource = fields;
        }

        public TerminalLineDto TerminalLine
        {
            get => terminalLineDto;
            set {
                terminalLineDto = value;
                SetTerminalLineFields(terminalLineDto.LineFieldDict);
            }
        }
    }
}
