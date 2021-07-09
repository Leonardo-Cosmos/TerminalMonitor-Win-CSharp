/* 2021/5/12 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using TerminalMonitor.Windows.Controls;

namespace TerminalMonitor.Matchers
{
    class TerminalLineMatcher
    {
        private readonly Condition matchCondition;

        public TerminalLineMatcher(Condition matchCondition)
        {
            this.matchCondition = matchCondition;
        }

        public bool IsMatch(TerminalLineDto terminalLineDto)
        {
            return IsMatch(terminalLineDto, matchCondition);
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, Condition matchCondition)
        {
            if (matchCondition is ConditionGroup conditionGroup)
            {
                return IsMatch(terminalLineDto, conditionGroup);
            }
            else if (matchCondition is FieldCondition condition)
            {
                return IsMatch(terminalLineDto, condition);
            }
            else
            {
                return false;
            }
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, ConditionGroup conditionGroup)
        {
            if (conditionGroup == null)
            {
                return false;
            }

            bool groupMatched = false;
            if (conditionGroup.MatchAny)
            {
                if (conditionGroup.Conditions == null)
                {
                    groupMatched = false;
                }
                else
                {
                    groupMatched = conditionGroup.Conditions.Any(condition => IsMatch(terminalLineDto, condition));
                }
            }
            else
            {
                if (conditionGroup.Conditions == null)
                {
                    groupMatched = true;
                }
                else
                {
                    groupMatched = conditionGroup.Conditions.All(condition => IsMatch(terminalLineDto, condition));
                }
            }

            return groupMatched ^ conditionGroup.Negative;
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, FieldCondition condition)
        {
            if (condition == null)
            {
                return false;
            }

            if (terminalLineDto.LineFieldDict == null || !terminalLineDto.LineFieldDict.ContainsKey(condition.FieldKey))
            {
                return condition.Negative;
            }

            var jsonProperty = terminalLineDto.LineFieldDict[condition.FieldKey];
            var fieldMatched = TextMatcher.IsMatch(jsonProperty.Text, condition.TargetValue, condition.MatchOperator);

            return fieldMatched ^ condition.Negative;
        }
    }
}
