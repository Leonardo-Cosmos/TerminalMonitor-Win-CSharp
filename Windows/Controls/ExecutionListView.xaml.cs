/* 2021/6/10 */
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
using TerminalMonitor.Execution;

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
            
        }

        private void Executor_ExecutionStarted(object sender, ExecutionInfoEventArgs e)
        {
            var executionName = e.Execution.Name;
            ExecutionListItemVO item = new()
            {
                Name = executionName,
            };
            executionVOs.Add(item);
        }

        private void Executor_ExecutionExited(object sender, ExecutionInfoEventArgs e)
        {
            var executionName = e.Execution.Name;
            ExecutionListItemVO item = executionVOs
                .FirstOrDefault(execution => execution.Name == executionName);
            if (item != null)
            {
                executionVOs.Remove(item);
            }
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
