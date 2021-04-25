/* 2021/4/22 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalMonitor.Checkers;

namespace TerminalMonitor.Windows.Controls
{
    class FilterItemVO : INotifyPropertyChanged
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

        private TextChecker.CheckOperator checkOperator;

        public TextChecker.CheckOperator CheckOperator
        {
            get { return checkOperator; }

            set
            {
                checkOperator = value;
                OnPropertyChanged();
            }
        }

        private string comparedValue;

        public string ComparedValue
        {
            get { return comparedValue; }
            set
            {
                comparedValue = value;
                OnPropertyChanged();
            }
        }
    }
}
