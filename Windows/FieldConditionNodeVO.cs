/* 2021/7/13 */
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Windows
{
    class FieldConditionNodeVO : ConditionNodeVO
    {
        private string fieldKey;

        public string FieldKey
        {
            get => fieldKey;
            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private TextMatchOperator matchOperator;

        public TextMatchOperator MatchOperator
        {
            get => matchOperator;
            set
            {
                matchOperator = value;
                OnPropertyChanged();
            }
        }

        private string targetValue;

        public string TargetValue
        {
            get => targetValue;
            set
            {
                targetValue = value;
                OnPropertyChanged();
            }
        }
    }
}
