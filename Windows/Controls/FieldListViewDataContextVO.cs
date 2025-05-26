/* 2021/11/6 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TerminalMonitor.Windows.Controls
{
    class FieldListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
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

        private bool isAnyFieldCutInClipboard;

        public bool IsAnyFieldCutInClipboard
        {
            get => isAnyFieldCutInClipboard;
            set
            {
                isAnyFieldCutInClipboard = value;
                OnPropertyChanged();
            }
        }

        public required ICommand AddCommand { get; init; }

        public required ICommand RemoveCommand { get; init; }

        public required ICommand EditCommand { get; init; }

        public required ICommand MoveLeftCommand { get; init; }

        public required ICommand MoveRightCommand { get; init; }

        public required ICommand CutCommand { get; init; }

        public required ICommand CopyCommand { get; init; }

        public required ICommand PasteCommnad { get; init; }
    }
}
