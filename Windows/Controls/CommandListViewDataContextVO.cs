/* 2021/11/2 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TerminalMonitor.Windows.Controls
{
    class CommandListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isAnyCommandSelected;

        public bool IsAnyCommandSelected
        {
            get => isAnyCommandSelected;
            set
            {
                isAnyCommandSelected = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; init; }

        public ICommand RemoveCommand { get; init; }

        public ICommand EditCommand { get; init; }

        public ICommand MoveUpCommand { get; init; }

        public ICommand MoveDownCommand { get; init; }

        public ICommand StartCommand { get; init; }
    }
}
