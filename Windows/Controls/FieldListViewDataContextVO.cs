/* 2021/11/6 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TerminalMonitor.Windows.Controls
{
    class FieldListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isAnyFieldSelected;

        public bool IsAnyFieldSelected
        {
            get => isAnyFieldSelected;
            set
            {
                isAnyFieldSelected = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyFieldInClipboard;

        public bool IsAnyFieldInClipboard
        {
            get => isAnyFieldInClipboard;
            set
            {
                isAnyFieldInClipboard = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; init; }

        public ICommand RemoveCommand { get; init; }

        public ICommand EditCommand { get; init; }

        public ICommand MoveLeftCommand { get; init; }

        public ICommand MoveRightCommand { get; init; }

        public ICommand CopyCommand { get; init; }

        public ICommand PasteCommnad { get; init; }
    }
}
