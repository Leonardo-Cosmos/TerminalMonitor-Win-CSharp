/* 2021/4/16 */
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
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
using TerminalMonitor.Parsers;
using TerminalMonitor.Settings;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TerminalMonitorSetting setting;

        private readonly ConcurrentQueue<string> terminalTextQueue = new ();
        private readonly ObservableCollection<TerminalListItem> terminalLines = new ();

        public MainWindow()
        {
            InitializeComponent();

            listTerminal.ItemsSource = terminalLines;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setting = SettingSerializer.Load() ?? new();

            var commandSetting = setting.Commands?[0] ?? new();
            textBoxCommand.Text = commandSetting.FilePath;

            var executionSetting = setting.Executions?[0] ?? new();
            textBoxArguments.Text = executionSetting.ArgumentsText;
            textBoxWorkDir.Text = executionSetting.WorkingDirectory;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CommandSetting commandSetting = new();
            commandSetting.FilePath = textBoxCommand.Text;
            setting.Commands = new List<CommandSetting>() { commandSetting };

            ExecutionSetting executionSetting = new();
            executionSetting.ArgumentsText = textBoxArguments.Text;
            executionSetting.WorkingDirectory = textBoxWorkDir.Text;
            setting.Executions = new List<ExecutionSetting>() { executionSetting };

            SettingSerializer.Save(setting);
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            var command = textBoxCommand.Text;
            var arguments = textBoxArguments.Text;
            var workDir = textBoxWorkDir.Text;
            var task = ExecuteCommand(command, arguments: arguments, workingDirectory: workDir);

            var timer = new DispatcherTimer();
            timer.Tick += (sender, e) => { 
            
                while (!terminalTextQueue.IsEmpty)
                {
                    if (terminalTextQueue.TryDequeue(out var text))
                    {
                        terminalLines.Add(JsonParser.ParseTerminalLine(text));
                    }
                }

                if (task.IsCompleted)
                {
                    timer.Stop();
                    terminalLines.Add(new() { PlainText = "Task is completed" });
                }

            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void ButtonBrowseWorkDir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() ?? false)
            {
                textBoxWorkDir.Text = dialog.SelectedPath;
            }
        }

        private async Task ExecuteCommand(string command, string arguments = null, string workingDirectory = null)
        {
            if (!String.IsNullOrEmpty(command))
            {
                var process = new Process();
                process.StartInfo.FileName = command;
                if (!String.IsNullOrWhiteSpace(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }
                if (!String.IsNullOrWhiteSpace(workingDirectory))
                {
                    process.StartInfo.WorkingDirectory = workingDirectory;
                }

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, e) =>
                {

                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        terminalTextQueue.Enqueue(e.Data);
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
