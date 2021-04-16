/* 2021/4/16 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace TerminalMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConcurrentQueue<string> terminalLineQueue = new ConcurrentQueue<string>();
        private ObservableCollection<string> terminalLines = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            listTerminal.ItemsSource = terminalLines;
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            var task = ExecuteCommand();

            var timer = new DispatcherTimer();
            timer.Tick += (sender, e) => { 
            
                while (!terminalLineQueue.IsEmpty)
                {
                    if (terminalLineQueue.TryDequeue(out var line))
                    {
                        terminalLines.Add(line);
                    }
                }

                if (task.IsCompleted)
                {
                    timer.Stop();
                }

            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private async Task ExecuteCommand()
        {
            var command = textBoxCommand.Text;

            if (String.IsNullOrEmpty(command))
            {
                var process = new Process();
                process.StartInfo.FileName = @"";
                process.StartInfo.WorkingDirectory = @"";
                process.StartInfo.Arguments = "";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, e) => {

                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        terminalLineQueue.Enqueue(e.Data);
                    }

                };

                process.Start();

                process.BeginOutputReadLine();
                
                await process.WaitForExitAsync();
                process.Close();
            }
        }
    }
}
