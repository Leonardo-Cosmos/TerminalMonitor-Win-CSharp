﻿/* 2021/5/30 */
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
using TerminalMonitor.Windows.ValidationRules;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for CommandDetailWindow.xaml
    /// </summary>
    public partial class CommandDetailWindow : Window
    {
        private readonly CommandDetailWindowDataContextVO dataContextVO = new()
        {
            Name = String.Empty,
        };

        private readonly List<string> existingCommandNames = [];

        private IEnumerable<string>? latestExistingCommandNames;

        private CommandConfig? command;

        public CommandDetailWindow()
        {
            InitializeComponent();

            DataContext = dataContextVO;

            Binding commandNameBinding = new("Name")
            {
                Source = dataContextVO,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            commandNameBinding.ValidationRules.Add(new UniqueItemRule()
            {
                ExistingValues = existingCommandNames,
                ErrorMessage = "Command name has been used already"
            });
            commandNameBinding.ValidationRules.Add(new NotEmptyRule()
            {
                ErrorMessage = "Command name should not be empty"
            });
            txtBxName.SetBinding(TextBox.TextProperty, commandNameBinding);
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
            existingCommandNames.Clear();
            if (latestExistingCommandNames != null)
            {
                existingCommandNames.AddRange(latestExistingCommandNames);
            }

            txtBxName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtBxCommand.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (Validation.GetHasError(txtBxName))
            {
                txtBxName.Focus();
                return;
            }

            if (Validation.GetHasError(txtBxCommand))
            {
                txtBxCommand.Focus();
                return;
            }

            SaveCommand();
            IsSaved = true;
            Close();
        }

        private void LoadCommand(CommandConfig? command)
        {
            this.command = command;
            IsSaved = false;

            if (command != null)
            {
                dataContextVO.Name = command.Name;
                dataContextVO.StartFile = command.StartFile;
                dataContextVO.Arguments = command.Arguments;
                dataContextVO.WorkDirectory = command.WorkDirectory;
            }
        }

        private void SaveCommand()
        {
            if (command != null)
            {
                command.Name = dataContextVO.Name;
                command.StartFile = dataContextVO.StartFile;
                command.Arguments = dataContextVO.Arguments;
                command.WorkDirectory = dataContextVO.WorkDirectory;
            }
            else
            {
                command = new()
                {
                    Name = dataContextVO.Name,
                    StartFile = dataContextVO.StartFile,
                    Arguments = dataContextVO.Arguments,
                    WorkDirectory = dataContextVO.WorkDirectory,
                };
            }
        }

        public bool IsSaved { get; set; }

        public IEnumerable<string> ExistingCommandNames
        {
            get
            {
                return new ReadOnlyCollection<string>([.. existingCommandNames]);
            }

            set
            {
                latestExistingCommandNames = value;
            }
        }

        public CommandConfig? Command
        {
            get => command;
            set => LoadCommand(value);
        }
    }
}
