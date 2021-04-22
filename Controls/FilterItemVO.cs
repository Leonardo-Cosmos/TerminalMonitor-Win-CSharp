/* 2021/4/22 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Controls
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

        private string compareOperator;

        public string CompareOperator
        {
            get { return compareOperator; }

            set
            {
                compareOperator = value;
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
