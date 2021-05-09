/* 2021/4/16 */
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerminalMonitor.Execution;
using TerminalMonitor.Settings;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TerminalMonitorSetting setting;
       
        public MainWindow()
        {
            InitializeComponent();
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
            CommandExecutor executor = new(command, arguments: arguments, workingDirectory: workDir);
            var execution = executor.Execute();

            terminalView.AddExecution(execution);
        }

        private void ButtonBrowseWorkDir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() ?? false)
            {
                textBoxWorkDir.Text = dialog.SelectedPath;
            }
        }
    }
}
