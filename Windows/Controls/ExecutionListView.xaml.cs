﻿/* 2021/6/10 */
using System;
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
using TerminalMonitor.Execution;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for ExecutionListView.xaml
    /// </summary>
    public partial class ExecutionListView : UserControl
    {
        private readonly ExecutionListViewDataContextVO dataContextVO = new();

        private readonly ObservableCollection<ExecutionListItemVO> executionVOs = new();

        private IExecutor executor;

        public ExecutionListView()
        {
            InitializeComponent();

            DataContext = dataContextVO;

            lstExecutions.ItemsSource = executionVOs;
        }

        private void LstExecutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstExecutions.SelectedItems.Count;
            dataContextVO.IsAnySelected = count > 0;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(StopExecution);
        }

        private void ForEachSelectedItem(Action<ExecutionListItemVO> action)
        {
            List<ExecutionListItemVO> itemVOs = new();
            foreach (var selectedItem in lstExecutions.SelectedItems)
            {
                if (selectedItem is ExecutionListItemVO itemVO)
                {
                    itemVOs.Add(itemVO);
                }
            }

            itemVOs.ForEach(action);
        }

        private void StopExecution(ExecutionListItemVO itemVO)
        {
            executor.Terminate(itemVO.Name);
        }

        private void UpdateExecutionInfo(ExecutionInfoEventArgs executionInfoEvent)
        {
            if (executionInfoEvent.Execution.Status == ExecutionStatus.Started)
            {
                var executionName = executionInfoEvent.Execution.Name;
                Debug.WriteLine($"Add {executionName} to list.");

                ExecutionListItemVO item = new()
                {
                    Name = executionName,
                };
                executionVOs.Add(item);
            }
            else if (executionInfoEvent.Execution.Status == ExecutionStatus.Completed)
            {
                var executionName = executionInfoEvent.Execution.Name;
                Debug.WriteLine($"Remove {executionName} from list.");

                ExecutionListItemVO item = executionVOs
                    .FirstOrDefault(execution => execution.Name == executionName);
                if (item != null)
                {
                    executionVOs.Remove(item);
                    Debug.WriteLine($"Removed {executionName} from list.");
                }
            }
            else if (executionInfoEvent.Execution.Status == ExecutionStatus.Error)
            {
                var executionName = executionInfoEvent.Execution.Name;
                Debug.WriteLine($"Remove {executionName} from list.");

                ExecutionListItemVO item = executionVOs
                    .FirstOrDefault(execution => execution.Name == executionName);
                if (item != null)
                {
                    executionVOs.Remove(item);
                    Debug.WriteLine($"Removed {executionName} from list.");
                }

                if (executionInfoEvent.Exception != null)
                {
                    Debug.WriteLine($"Execution exception: {executionInfoEvent.Exception.Message}");
                    Debug.WriteLine(executionInfoEvent.Exception.StackTrace);
                    MessageBox.Show(executionInfoEvent.Exception.Message, "Error during execution",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Executor_ExecutionStarted(object sender, ExecutionInfoEventArgs e)
        {
            this.Dispatcher.Invoke(() => this.UpdateExecutionInfo(e));
        }

        private void Executor_ExecutionExited(object sender, ExecutionInfoEventArgs e)
        {
            this.Dispatcher.Invoke(() => this.UpdateExecutionInfo(e));
        }

        public IExecutor Executor
        {
            get => executor;
            set
            {
                if (executor != value && executor != null)
                {
                    executor.ExecutionStarted -= Executor_ExecutionStarted;
                    executor.ExecutionExited -= Executor_ExecutionExited;
                }

                executor = value;

                if (executor != null)
                {
                    executor.ExecutionStarted += Executor_ExecutionStarted;
                    executor.ExecutionExited += Executor_ExecutionExited;
                }
            }
        }
    }
}
