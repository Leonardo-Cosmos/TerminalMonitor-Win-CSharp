/* 2021/5/30 */
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
        private readonly ObservableCollection<CommandListItemVO> commandVOs = new();

        private readonly List<CommandConfig> commands = new();

        public CommandListView()
        {
            InitializeComponent();

            lstCommands.ItemsSource = commandVOs;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var existingCommandNames = commands.Select(config => config.Name);
            CommandDetailWindow window = new()
            {
                ExistingCommandNames = existingCommandNames,
            };
            if (window.ShowDialog() ?? false)
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

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            if (lstCommands.SelectedItem is CommandListItemVO selectedItem)
            {
                var index = commandVOs.IndexOf(selectedItem);

                var command = commands[index];
                var existingCommandNames = commands
                    .Select(command => command.Name)
                    .Where(commandName => commandName != command.Name);
                CommandDetailWindow window = new()
                {
                    Command = command,
                    ExistingCommandNames = existingCommandNames,
                };
                if (window.ShowDialog() ?? false)
                {
                    command = window.Command;
                    commands[index] = command;

                    commandVOs[index].Name = command.Name;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstCommands.SelectedItem is CommandListItemVO selectedItem)
            {
                var index = commandVOs.IndexOf(selectedItem);
                commandVOs.RemoveAt(index);

                commands.RemoveAt(index);
            }
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (lstCommands.SelectedItem is CommandListItemVO selectedItem)
            {
                var index = commandVOs.IndexOf(selectedItem);
                if (index > 0)
                {
                    commandVOs.RemoveAt(index);
                    commandVOs.Insert(index - 1, selectedItem);

                    lstCommands.SelectedItem = selectedItem;

                    var command = commands[index];
                    commands.RemoveAt(index);
                    commands.Insert(index - 1, command);
                }
            }
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (lstCommands.SelectedItem is CommandListItemVO selectedItem)
            {
                var index = commandVOs.IndexOf(selectedItem);
                if (index < commandVOs.Count - 1)
                {
                    commandVOs.RemoveAt(index);
                    commandVOs.Insert(index + 1, selectedItem);

                    lstCommands.SelectedItem = selectedItem;

                    var command = commands[index];
                    commands.RemoveAt(index);
                    commands.Insert(index + 1, command);
                }
            }
        }

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
