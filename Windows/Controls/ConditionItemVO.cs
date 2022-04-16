/* 2021/4/22 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Windows.Controls
{
    class ConditionItemVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Id { get; init; }

        private string fieldKey;

        public string FieldKey
        {
            get { return fieldKey; }

            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private TextMatchOperator matchOperator;

        public TextMatchOperator MatchOperator
        {
            get { return matchOperator; }

            set
            {
                matchOperator = value;
                OnPropertyChanged();
            }
        }

        private string targetValue;

        public string TargetValue
        {
            get { return targetValue; }
            set
            {
                targetValue = value;
                OnPropertyChanged();
            }
        }

        private string conditionName;

        public string ConditionName
        {
            get { return conditionName; }
            set
            {
                conditionName = value;
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
    }
}
