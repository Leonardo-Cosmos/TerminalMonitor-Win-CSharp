/* 2021/7/31 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Windows.Controls
{
    class ConditionListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isAnyConditionSelected;

        public bool IsAnyConditionSelected
        {
            get => isAnyConditionSelected;
            set
            {
                isAnyConditionSelected = value;
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

        private bool isAnyConditionCutInClipboard;

        public bool IsAnyConditionCutInClipboard
        {
            get => isAnyConditionCutInClipboard;
            set
            {
                isAnyConditionCutInClipboard = value;
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

        public required GroupMatchMode MatchMode
        {
            get => matchMode;
            set { matchMode = value; OnPropertyChanged(); }
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
