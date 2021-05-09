/* 2021/4/20 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows
{
    class TerminalTextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string PlainText { get; set; }

        public List<TerminalTextFieldVO> ParsedFields { get; set; }

        public bool IsMatch(IEnumerable<FilterCondition> filterConditions)
        {
            if (filterConditions == null)
            {
                return true;
            }

            bool included = filterConditions
                .Where(filterCondition => !filterCondition.Excluded)
                .All(filterCondition => IsMatch(filterCondition.Condition));

            bool excluded = filterConditions
                .Where(filterConditions => filterConditions.Excluded)
                .Any(filterCondition => IsMatch(filterCondition.Condition));

            return included && !excluded;
        }

        private bool IsMatch(TextCondition condition)
        {
            if (condition == null)
            {
                return false;
            }
            var field = (ParsedFields ?? new()).FirstOrDefault(field => field.Key == condition.FieldKey);
            return TextMatcher.IsMatch(field.Value, condition.TargetValue, condition.MatchOperator);
        }

    }
}
