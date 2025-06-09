/* 2021/11/5 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TerminalMonitor.Windows.Controls
{
    class ExecutionListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isAnySelected;

        public bool IsAnyExecutionSelected
        {
            get => isAnySelected;
            set
            {
                isAnySelected = value;
                OnPropertyChanged();
            }
        }

        public required ICommand StopCommand { get; init; }

        public required ICommand RestartCommand { get; init; }
    }
}
