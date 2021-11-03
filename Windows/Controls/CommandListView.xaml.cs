/* 2021/5/30 */
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
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for CommandListView.xaml
    /// </summary>
    public partial class CommandListView : UserControl
    {
        private readonly CommandListViewDataContextVO dataContext = new();

        private readonly ObservableCollection<CommandListItemVO> commandVOs = new();

        private readonly List<CommandConfig> commands = new();

        public CommandListView()
        {
            InitializeComponent();

            DataContext = dataContext;

            lstCommands.ItemsSource = commandVOs;
        }

        private void LstCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstCommands.SelectedItems.Count;
            dataContext.IsSingleSelected = count == 1;
            dataContext.IsAnySelected = count > 0;
        }

        private void LstCommands_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ForSelectedItem(ModifyCommand);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCommand();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(DeleteCommand);
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(ModifyCommand);
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(MoveCommandUp, byOrder: true, recoverSelection: true);
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(MoveCommandDown, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(StartCommand);
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
            List<CommandListItemVO> itemVOs = new();
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

        private void AddCommand()
        {
            var existingCommandNames = commands.Select(config => config.Name);
            CommandDetailWindow window = new()
            {
                ExistingCommandNames = existingCommandNames,
            };

            window.Closing += AddCommandWindow_Closing;
            window.Show();
        }

        private void AddCommandWindow_Closing(object sender, CancelEventArgs e)
        {
            var window = (CommandDetailWindow)sender;
            if (window.Saved)
            {
                var command = window.Command;

                CommandListItemVO item = new()
                {
                    Name = command.Name,
                };
                commandVOs.Add(item);
                lstCommands.SelectedItem = item;

                commands.Add(command);
            }
        }

        private void DeleteCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);
            commandVOs.RemoveAt(index);

            commands.RemoveAt(index);
        }

        private void ModifyCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);

            var command = commands[index];
            var existingCommandNames = commands
                .Select(command => command.Name)
                .Where(commandName => commandName != command.Name);
            CommandDetailWindow window = new()
            {
                Command = command,
                ExistingCommandNames = existingCommandNames,
            };

            window.Closing += (sender, e) =>
            {
                itemVO.Name = command.Name;
            };

            window.Show();
        }

        private void MoveCommandUp(CommandListItemVO itemVO)
        {
            var srcIndex = commandVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex - 1 + commandVOs.Count) % commandVOs.Count;

            commandVOs.RemoveAt(srcIndex);
            commandVOs.Insert(dstIndex, itemVO);

            lstCommands.SelectedItem = itemVO;

            var command = commands[srcIndex];
            commands.RemoveAt(srcIndex);
            commands.Insert(dstIndex, command);
        }

        private void MoveCommandDown(CommandListItemVO itemVO)
        {
            var srcIndex = commandVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex + 1) % commandVOs.Count;

            commandVOs.RemoveAt(srcIndex);
            commandVOs.Insert(dstIndex, itemVO);

            lstCommands.SelectedItem = itemVO;

            var command = commands[srcIndex];
            commands.RemoveAt(srcIndex);
            commands.Insert(dstIndex, command);
        }

        private void StartCommand(CommandListItemVO itemVO)
        {
            var index = commandVOs.IndexOf(itemVO);

            var command = commands[index];
            OnCommandStarted(new()
            {
                Command = command
            });
        }

        protected void OnCommandStarted(CommandRunEventArgs e)
        {
            CommandStarted?.Invoke(this, e);
        }

        public event CommandRunEventHandler CommandStarted;

        public IEnumerable<CommandConfig> Commands
        {
            get
            {
                return new ReadOnlyCollection<CommandConfig>(commands.ToArray());
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
                }).ToList()
                .ForEach(commandVO => commandVOs.Add(commandVO));
            }
        }
    }
}
