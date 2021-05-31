/* 2021/5/30 */
using Ookii.Dialogs.Wpf;
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
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for CommandDetailWindow.xaml
    /// </summary>
    public partial class CommandDetailWindow : Window
    {
        private readonly CommandDetailWindowDataContextVO dataContextVO = new();

        private readonly List<string> existingCommandNames = new();

        public CommandDetailWindow()
        {
            InitializeComponent();

            DataContext = dataContextVO;
        }

        private void BtnBrowseWorkDir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                SelectedPath = dataContextVO.WorkDirectory,
            };
            if (dialog.ShowDialog() ?? false)
            {
                dataContextVO.WorkDirectory = dialog.SelectedPath;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        public IEnumerable<string> ExistingCommandNames
        {
            get
            {
                return new ReadOnlyCollection<string>(existingCommandNames.ToArray());
            }

            set
            {
                existingCommandNames.Clear();
                if (value != null)
                {
                    existingCommandNames.AddRange(value);
                }
            }
        }

        public CommandConfig Command
        {
            get
            {
                return new() { 
                    StartFile = dataContextVO.StartFile,
                    Arguments = dataContextVO.Arguments,
                    WorkDirectory = dataContextVO.WorkDirectory,
                };
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                dataContextVO.StartFile = value.StartFile;
                dataContextVO.Arguments = value.Arguments;
                dataContextVO.WorkDirectory = value.WorkDirectory;
            }
        }
    }
}
