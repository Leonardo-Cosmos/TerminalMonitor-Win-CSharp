/* 2021/7/31 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Windows.Controls
{
    class FilterListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isAnySelected;

        public bool IsAnySelected
        {
            get => isAnySelected;
            set
            {
                isAnySelected = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyConditionInClipboard;

        public bool IsAnyConditionInClipboard
        {
            get => isAnyConditionInClipboard;
            set
            {
                isAnyConditionInClipboard = value;
                OnPropertyChanged();
            }
        }

        private bool isInverted;

        public bool IsInverted
        {
            get => isInverted;
            set { isInverted = value; OnPropertyChanged(); }
        }

        private bool defaultResult;

        public bool DefaultResult
        {
            get => defaultResult;
            set { defaultResult = value; OnPropertyChanged(); }
        }

        private bool isDisabled;

        public bool IsDisabled
        {
            get => isDisabled;
            set { isDisabled = value; OnPropertyChanged(); }
        }

        private GroupMatchMode matchMode;

        public GroupMatchMode MatchMode
        {
            get => matchMode;
            set { matchMode = value; OnPropertyChanged(); }
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
