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

            var conditions = conditionGroup.Conditions?.Where(condition => !(condition?.DismissMatch ?? true)) ?? new List<Condition>();

            bool groupMatched;
            if (conditionGroup.DismissMatch)
            {
                groupMatched = conditionGroup.DefaultMatch;
            }
            else if (!conditions.Any())
            {
                groupMatched = conditionGroup.DefaultMatch;
            }
            else
            {
                groupMatched = conditionGroup.MatchMode switch
                {
                    GroupMatchMode.All => conditions.All(condition => IsMatch(terminalLineDto, condition)),
                    GroupMatchMode.Any => conditions.Any(condition => IsMatch(terminalLineDto, condition)),
                    _ => false
                };
            }

            return groupMatched ^ conditionGroup.NegativeMatch;
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, FieldCondition condition)
        {
            if (condition == null)
            {
                return false;
            }

            bool fieldMatched;
            if (condition.DismissMatch)
            {
                fieldMatched = condition.DefaultMatch;
            }
            else if (terminalLineDto.LineFieldDict == null || !terminalLineDto.LineFieldDict.ContainsKey(condition.FieldKey))
            {
                fieldMatched = condition.DefaultMatch;
            }
            else
            {
                var jsonProperty = terminalLineDto.LineFieldDict[condition.FieldKey];
                fieldMatched = TextMatcher.IsMatch(jsonProperty.Text, condition.TargetValue, condition.MatchOperator);
            }

            return fieldMatched ^ condition.NegativeMatch;
        }
    }
}
