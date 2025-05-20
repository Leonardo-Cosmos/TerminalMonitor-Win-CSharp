/* 2021/7/13 */
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Windows
{
    class FieldConditionNodeVO : ConditionNodeVO
    {
        private string? fieldKey;

        public required string FieldKey
        {
            get => fieldKey!;
            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private TextMatchOperator matchOperator;

        public required TextMatchOperator MatchOperator
        {
            get => matchOperator;
            set
            {
                matchOperator = value;
                OnPropertyChanged();
            }
        }

        private string? targetValue;

        public required string TargetValue
        {
            get => targetValue!;
            set
            {
                targetValue = value;
                OnPropertyChanged();
            }
        }
    }
}
