/* 2021/5/30 */
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using TerminalMonitor.Execution;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for CommandListView.xaml
    /// </summary>
    public partial class CommandListView : UserControl
    {
        private readonly CommandListViewDataContextVO dataContextVO;

        private readonly ObservableCollection<CommandListItemVO> commandVOs = [];

        private readonly List<CommandConfig> commands = [];

        private IExecutor? executor;

        public CommandListView()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                AddCommand = new RelayCommand(AddCommand, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedCommands, () => dataContextVO!.IsAnyCommandSelected),
                EditCommand = new RelayCommand(EditSelectedCommands, () => dataContextVO!.IsAnyCommandSelected),
                MoveUpCommand = new RelayCommand(MoveSelectedCommandsUp, () => dataContextVO!.IsAnyCommandSelected),
                MoveDownCommand = new RelayCommand(MoveSelectedCommandsDown, () => dataContextVO!.IsAnyCommandSelected),
                StartCommand = new RelayCommand(StartSelectedCommands, () => dataContextVO!.IsAnyCommandSelected),
                StopCommand = new RelayCommand(StopSelectedCommands, () => dataContextVO!.IsAnyCommandSelected),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            lstCommands.ItemsSource = commandVOs;
        }

        private void DataContextVO_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CommandListViewDataContextVO.IsAnyCommandSelected):
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveUpCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveDownCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.StartCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.StopCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstCommands.SelectedItems.Count;
            dataContextVO.IsAnyCommandSelected = count > 0;
        }

        private void LstCommands_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitResult.VisualHit.GetType() != typeof(CommandListItemVO))
            {
                lstCommands.UnselectAll();
            }
        }

        private void LstCommands_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelectedCommand();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.Tag!;
            var commandId = (Guid)tag;
            var commandVO = commandVOs.First(itemVO => itemVO.Id == commandId);
            StartCommand(commandVO);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.Tag!;
            var commandId = (Guid)tag;
            var commandVO = commandVOs.First(itemVO => itemVO.Id == commandId);
            StopCommand(commandVO);
        }

        private void ForSelectedItem(Action<CommandListItemVO> action)
        {
            if (lstCommands.SelectedItem is CommandListItemVO itemVO)
            {
                action(itemVO);
            }
        }

        private void ForEachSelectedItem(Action<CommandListItemVO> action,
            bool byOrder = false, bool reverseOrder = false, bool recoverSelection = false)
        {
            List<CommandListItemVO> itemVOs = [];
            foreach (var selectedItem in lstCommands.SelectedItems)
            {
                if (selectedItem is CommandListItemVO itemVO)
                {
                    itemVOs.Add(itemVO);
                }
            }

            if (byOrder)
            {
                itemVOs.Sort((itemX, itemY) =>
                    commandVOs.IndexOf(itemX) - commandVOs.IndexOf(itemY));
            }

            if (reverseOrder)
            {
                itemVOs.Reverse();
            }

            itemVOs.ForEach(action);

            if (recoverSelection)
            {
                itemVOs.ForEach(itemVO => lstCommands.SelectedItems.Add(itemVO));
            }
        }

        private void InsertAtSelectedItem(params (CommandConfig commandConfig, CommandListItemVO itemVO)[] commandTuples)
        {
            var selectedIndex = lstCommands.SelectedIndex;
            if (selectedIndex == -1)
            {
                foreach (var (commandConfig, itemVO) in commandTuples)
                {
                    commandVOs.Add(itemVO);
                    lstCommands.SelectedItems.Add(itemVO);

                    commands.Add(commandConfig);
                }
            }
            else
            {
                lstCommands.SelectedItems.Clear();

                var reversedConditionTuples = commandTuples.Reverse().ToArray();
                foreach (var (commandConfig, itemVO) in reversedConditionTuples)
                {
                    commandVOs.Insert(selectedIndex, itemVO);
                    lstCommands.SelectedItems.Add(itemVO);

                    commands.Insert(selectedIndex, commandConfig);
                }
            }
        }

        private void AddCommand()
        {
            var existingCommandNames = commands.Select(command => command.Name);
            CommandDetailWindow window = new()
            {
                ExistingCommandNames = existingCommandNames,
            };

            window.Closing += (object? sender, CancelEventArgs e) =>
            {
                if (window.IsSaved && window.Command != null)
                {
                    var commandConfig = window.Command;

                    CommandListItemVO itemVO = new()
                    {
                        Name = commandConfig.Name,
                        Id = commandConfig.Id,
                    };

                    InsertAtSelectedItem((commandConfig, itemVO));
                }
            };

            window.Show();
        }

        private void RemoveSelectedCommands()
        {
            ForEachSelectedItem(RemoveCommand);
        }

        private void RemoveCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);
            commandVOs.RemoveAt(index);

            commands.RemoveAt(index);
        }

        private void EditSelectedCommand()
        {
            ForSelectedItem(EditCommand);
        }

        private void EditSelectedCommands()
        {
            ForEachSelectedItem(EditCommand);
        }

        private void EditCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);

            var commandConfig = commands[index];
            var existingCommandNames = commands
                .Select(command => command.Name)
                .Where(commandName => commandName != commandConfig.Name);
            CommandDetailWindow window = new()
            {
                Command = commandConfig,
                ExistingCommandNames = existingCommandNames,
            };

            window.Closing += (object? sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    itemVO.Name = commandConfig.Name;
                }
            };

            window.Show();
        }

        private void MoveSelectedCommandsUp()
        {
            ForEachSelectedItem(MoveCommandUp, byOrder: true, recoverSelection: true);
        }

        private void MoveCommandUp(CommandListItemVO itemVO)
        {
            var srcIndex = commandVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex - 1 + commandVOs.Count) % commandVOs.Count;

            commandVOs.RemoveAt(srcIndex);
            commandVOs.Insert(dstIndex, itemVO);

            var commandConfig = commands[srcIndex];
            commands.RemoveAt(srcIndex);
            commands.Insert(dstIndex, commandConfig);
        }

        private void MoveSelectedCommandsDown()
        {
            ForEachSelectedItem(MoveCommandDown, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void MoveCommandDown(CommandListItemVO itemVO)
        {
            var srcIndex = commandVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex + 1) % commandVOs.Count;

            commandVOs.RemoveAt(srcIndex);
            commandVOs.Insert(dstIndex, itemVO);

            var commandConfig = commands[srcIndex];
            commands.RemoveAt(srcIndex);
            commands.Insert(dstIndex, commandConfig);
        }

        private void StartSelectedCommands()
        {
            ForEachSelectedItem(StartCommand);
        }

        private void StartCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);

            var command = commands[index];
            executor?.Execute(command);
        }

        private void StopSelectedCommands()
        {
            ForEachSelectedItem(StopCommand);
        }

        private void StopCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);

            var command = commands[index];
            executor?.TerminateAll(command.Id);
        }

        private void Executor_CommandFirstExecutionStarted(object sender, CommandInfoEventArgs e)
        {
            var commandId = e.Command.Id;
            var commandVO = commandVOs
                .First(commandVO => commandVO.Id == commandId);

            if (commandVO != null)
            {
                this.Dispatcher.Invoke(() => commandVO.IsRunning = true);
            }
        }

        private void Executor_CommandLastExecutionExited(object sender, CommandInfoEventArgs e)
        {
            var commandId = e.Command.Id;
            var commandVO = commandVOs
                .First(commandVO => commandVO.Id == commandId);

            if (commandVO != null)
            {
                this.Dispatcher.Invoke(() => commandVO.IsRunning = false);
            }
        }

        public IEnumerable<CommandConfig>? Commands
        {
            get
            {
                return new ReadOnlyCollection<CommandConfig>([.. commands]);
            }

            set
            {
                commands.Clear();
                commandVOs.Clear();
                if (value == null)
                {
                    return;
                }

                commands.AddRange(value);
                value.Select(command => new CommandListItemVO()
                {
                    Name = command.Name,
                    Id = command.Id,
                }).ToList()
                .ForEach(commandVO => commandVOs.Add(commandVO));
            }
        }

        public IExecutor? Executor
        {
            get => executor;
            set
            {
                if (executor == value)
                {
                    return;
                }
                
                if (executor != null)
                {
                    executor.CommandFirstExecutionStarted -= Executor_CommandFirstExecutionStarted;
                    executor.CommandLastExecutionExited -= Executor_CommandLastExecutionExited;
                }

                executor = value;

                if (executor != null)
                {
                    executor.CommandFirstExecutionStarted += Executor_CommandFirstExecutionStarted;
                    executor.CommandLastExecutionExited += Executor_CommandLastExecutionExited;
                }
            }
        }
    }
}
