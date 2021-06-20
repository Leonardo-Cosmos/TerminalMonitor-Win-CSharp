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

            commandListView.CommandRun += (sender, e) =>
            {
                commandExecutor.Execute(e.Command);
            };

            executionListView.Executor = commandExecutor;
            terminalTabControl.LineProducer = commandExecutor;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setting = SettingSerializer.Load() ?? new();

            commandListView.Commands = setting.Commands?
                .Select(command => CommandConfigSettings.Load(command));

            var terminalSetting = setting.Terminals?[0] ?? new();
            //terminalView.VisibleFields = terminalSetting.Fields?
            //    .Select(field => FieldDisplayDetailSettings.Load(field));
            //terminalView.FilterConditions = terminalSetting.Filters?
            //    .Select(filter => FilterConditionSettings.Load(filter));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            commandExecutor.TerminateAll();

            setting.Commands = commandListView.Commands
                .Select(command => CommandConfigSettings.Save(command)).ToList();

            TerminalSetting terminalSetting = new();
            //terminalSetting.Fields = terminalView.VisibleFields
            //    .Select(field => FieldDisplayDetailSettings.Save(field)).ToList();
            //terminalSetting.Filters = terminalView.FilterConditions
            //    .Select(filter => FilterConditionSettings.Save(filter)).ToList();
            //setting.Terminals = new List<TerminalSetting>() { terminalSetting };

            SettingSerializer.Save(setting);
        }
    }
}
