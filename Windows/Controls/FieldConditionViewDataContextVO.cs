/* 2021/5/23 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Windows.Controls
{
    class FieldConditionViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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

        public bool defaultMatch;

        public bool DefaultMatch
        {
            get { return defaultMatch; }
            set
            {
                defaultMatch = value;
                OnPropertyChanged();
            }
        }

        public bool ignored;

        public bool Ignored
        {
            get { return ignored; }
            set
            {
                ignored = value;
                OnPropertyChanged();
            }
        }
    }
}
