/* 2021/6/10 */
using Microsoft.Toolkit.Mvvm.Input;
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
        private readonly ExecutionListViewDataContextVO dataContextVO;

        private readonly ObservableCollection<ExecutionListItemVO> executionVOs = [];

        private IExecutor? executor;

        public ExecutionListView()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                StopCommand = new RelayCommand(StopSelectedExecutions, () => dataContextVO!.IsAnyExecutionSelected),
                RestartCommand = new RelayCommand(RestartSelectedExecutions, () => dataContextVO!.IsAnyExecutionSelected),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            lstExecutions.ItemsSource = executionVOs;
        }

        private void DataContextVO_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ExecutionListViewDataContextVO.IsAnyExecutionSelected):
                    (dataContextVO.StopCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstExecutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstExecutions.SelectedItems.Count;
            dataContextVO.IsAnyExecutionSelected = count > 0;
        }

        private void LstExecutions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitResult.VisualHit.GetType() != typeof(ExecutionListItemVO))
            {
                lstExecutions.UnselectAll();
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.Tag!;
            var executionId = (Guid)tag;
            executor?.Terminate(executionId);
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button)?.Tag!;
            var executionId = (Guid)tag;
            executor?.Restart(executionId);
        }

        private void ForEachSelectedItem(Action<ExecutionListItemVO> action)
        {
            List<ExecutionListItemVO> itemVOs = [];
            foreach (var selectedItem in lstExecutions.SelectedItems)
            {
                if (selectedItem is ExecutionListItemVO itemVO)
                {
                    itemVOs.Add(itemVO);
                }
            }

            itemVOs.ForEach(action);
        }

        private void StopSelectedExecutions()
        {
            ForEachSelectedItem(StopExecution);
        }

        private void StopExecution(ExecutionListItemVO itemVO)
        {
            executor?.Terminate(itemVO.Id);
        }

        private void RestartSelectedExecutions()
        {
            ForEachSelectedItem(RestartExecution);
        }

        private void RestartExecution(ExecutionListItemVO itemVO)
        {
            executor?.Restart(itemVO.Id);
        }

        private void UpdateExecutionInfo(ExecutionInfoEventArgs executionInfoEvent)
        {
            if (executionInfoEvent.Execution.Status == ExecutionStatus.Started)
            {
                var executionName = executionInfoEvent.Execution.Name;
                var executionId = executionInfoEvent.Execution.Id;
                Debug.WriteLine($"Add execution (name: {executionName}, id: {executionId}) to list.");

                ExecutionListItemVO item = new()
                {
                    Name = executionName,
                    Id = executionId,
                };
                executionVOs.Add(item);
                Debug.WriteLine($"Added execution (name: {executionName}, id: {executionId}) to list");
            }
            else if (executionInfoEvent.Execution.Status == ExecutionStatus.Completed)
            {
                var executionName = executionInfoEvent.Execution.Name;
                var executionId = executionInfoEvent.Execution.Id;
                Debug.WriteLine($"Remove execution (name: {executionName}, id: {executionId}) (normal) from list.");

                ExecutionListItemVO? item = executionVOs
                    .FirstOrDefault(execution => execution.Id == executionId);
                if (item != null)
                {
                    executionVOs.Remove(item);
                    Debug.WriteLine($"Removed execution (name: {executionName}, id: {executionId}) (normal) from list.");
                }
                else
                {
                    Debug.WriteLine($"Remove execution (name: {executionName}, id: {executionId}) (normal) skipped because it is not found.");
                }
            }
            else if (executionInfoEvent.Execution.Status == ExecutionStatus.Error)
            {
                var executionName = executionInfoEvent.Execution.Name;
                var executionId = executionInfoEvent.Execution.Id;
                Debug.WriteLine($"Remove execution (name: {executionName}, id: {executionId}) (error) from list.");

                ExecutionListItemVO? item = executionVOs
                    .FirstOrDefault(execution => execution.Id == executionId);
                if (item != null)
                {
                    executionVOs.Remove(item);
                    Debug.WriteLine($"Removed execution (name: {executionName}, id: {executionId}) (error) from list.");
                }
                else
                {
                    Debug.WriteLine($"Remove execution (name: {executionName}, id: {executionId}) (error) skipped because it is not found.");
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
