/* 2021/5/23 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Windows.Controls
{
    class FieldConditionViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? fieldKey;

        public required string FieldKey
        {
            get { return fieldKey!; }
            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private TextMatchOperator matchOperator;

        public required TextMatchOperator MatchOperator
        {
            get { return matchOperator; }
            set
            {
                matchOperator = value;
                OnPropertyChanged();
            }
        }

        private string? targetValue;

        public required string TargetValue
        {
            get { return targetValue!; }
            set
            {
                targetValue = value;
                OnPropertyChanged();
            }
        }

        private bool isInverted;

        public bool IsInverted
        {
            get { return isInverted; }
            set
            {
                isInverted = value;
                OnPropertyChanged();
            }
        }

        private bool defaultResult;

        public bool DefaultResult
        {
            get { return defaultResult; }
            set
            {
                defaultResult = value;
                OnPropertyChanged();
            }
        }

        private bool isDisabled;

        public bool IsDisabled
        {
            get { return isDisabled; }
            set
            {
                isDisabled = value;
                OnPropertyChanged();
            }
        }
    }
}
