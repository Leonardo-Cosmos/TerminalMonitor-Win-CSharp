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

        public bool IsMatch(TerminalLineDto terminalLineDto)
        {
            return IsMatch(terminalLineDto, filterConditions);
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, IEnumerable<FilterCondition> filterConditions)
        {
            if (filterConditions == null || !filterConditions.Any())
            {
                return true;
            }

            bool included = filterConditions
                .Where(filterCondition => !filterCondition.Excluded)
                .All(filterCondition => IsMatch(terminalLineDto, filterCondition.Condition));

            bool excluded = filterConditions
                .Where(filterConditions => filterConditions.Excluded)
                .Any(filterCondition => IsMatch(terminalLineDto, filterCondition.Condition));

            return included && !excluded;
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, TextCondition condition)
        {
            if (condition == null)
            {
                return false;
            }
            
            if (terminalLineDto.LineFieldDict == null || !terminalLineDto.LineFieldDict.ContainsKey(condition.FieldKey))
            {
                return false;
            }

            var jsonProperty = terminalLineDto.LineFieldDict[condition.FieldKey];

            return TextMatcher.IsMatch(jsonProperty.Text, condition.TargetValue, condition.MatchOperator);
        }
    }
}
