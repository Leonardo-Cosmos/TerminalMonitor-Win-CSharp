/* 2021/4/16 */
using Newtonsoft.Json;
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
using TerminalMonitor.Settings.Models;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandExecutor commandExecutor = new();

        private TerminalMonitorSetting setting;

        public MainWindow()
        {
            InitializeComponent();

            commandListView.CommandStarted += (sender, e) =>
            {
                commandExecutor.Execute(e.Command);
            };

            executionListView.Executor = commandExecutor;
            terminalTabControl.TerminalLineProducer = commandExecutor;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setting = SettingSerializer.Load() ?? new();

            commandListView.Commands = setting.Commands?
                .Select(command => CommandConfigSettings.Load(command));

            terminalTabControl.Terminals = setting.Terminals?
                .Select(terminal => TerminalConfigSettings.Load(terminal));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            commandExecutor.Shutdown();

            setting.Commands = commandListView.Commands
                .Select(command => CommandConfigSettings.Save(command)).ToList();

            setting.Terminals = terminalTabControl.Terminals
                .Select(terminal => TerminalConfigSettings.Save(terminal)).ToList();

            SettingSerializer.Save(setting);

            Properties.WindowSettings.Default.Save();
            Properties.TerminalSettings.Default.Save();
        }
    }
}
