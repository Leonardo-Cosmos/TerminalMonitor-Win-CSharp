/* 2021/5/12 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;
using TerminalMonitor.Windows.Controls;

namespace TerminalMonitor.Matchers
{
    class TerminalLineMatcher
    {
        private readonly IEnumerable<FilterCondition> filterConditions;

        public TerminalLineMatcher(IEnumerable<FilterCondition> filterConditions)
        {
            this.filterConditions = filterConditions;
        }

        public bool IsMatch(TerminalLineVO terminalLineVO)
        {
            return IsMatch(terminalLineVO, filterConditions);
        }

        public static bool IsMatch(TerminalLineVO terminalLineVO, IEnumerable<FilterCondition> filterConditions)
        {
            if (filterConditions == null)
            {
                return true;
            }

            bool included = filterConditions
                .Where(filterCondition => !filterCondition.Excluded)
                .All(filterCondition => IsMatch(terminalLineVO, filterCondition.Condition));

            bool excluded = filterConditions
                .Where(filterConditions => filterConditions.Excluded)
                .Any(filterCondition => IsMatch(terminalLineVO, filterCondition.Condition));

            return included && !excluded;
        }

        public static bool IsMatch(TerminalLineVO terminalLineVO, TextCondition condition)
        {
            if (condition == null)
            {
                return false;
            }
            var field = (terminalLineVO.ParsedFields ?? new()).FirstOrDefault(field => field.Key == condition.FieldKey);
            if (field == null)
            {
                return false;
            }

            return TextMatcher.IsMatch(field.Value, condition.TargetValue, condition.MatchOperator);
        }
    }
}
