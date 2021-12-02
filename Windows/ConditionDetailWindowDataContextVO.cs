/* 2021/7/28 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Windows
{
    class ConditionDetailWindowDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isConditionSelected;

        public bool IsConditionSelected
        {
            get => isConditionSelected;
            set
            {
                isConditionSelected = value;
                OnPropertyChanged();
            }
        }

        private bool isConditionInClipboard;

        public bool IsConditionInClipboard
        {
            get => isConditionInClipboard;
            set
            {
                isConditionInClipboard = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyConditionCutInClipboard;

        public bool IsConditionCutInClipboard
        {
            get => isAnyConditionCutInClipboard;
            set
            {
                isAnyConditionCutInClipboard = value;
                OnPropertyChanged();
            }
        }

        private string conditionName;

        public string ConditionName
        {
            get => conditionName;
            set { conditionName = value; OnPropertyChanged(); }
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

        public ICommand AddFieldCommand { get; init; }

        public ICommand AddGroupCommand { get; init; }

        public ICommand RemoveCommand { get; init; }

        public ICommand MoveUpCommand { get; init; }

        public ICommand MoveDownCommand { get; init; }

        public ICommand CutCommand { get; init; }

        public ICommand CopyCommand { get; init; }

        public ICommand PasteCommnad { get; init; }
    }
}
