/* 2021/6/10 */
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
        private readonly ObservableCollection<ExecutionListItemVO> executionVOs = new();

        private IExecutor executor;

        public ExecutionListView()
        {
            InitializeComponent();

            lstExecutions.ItemsSource = executionVOs;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button).Tag;
            var executionName = tag as string;
            executor.Terminate(executionName);
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
