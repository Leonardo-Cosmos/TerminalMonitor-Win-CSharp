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
            if (matchCondition is GroupCondition groupCondition)
            {
                return IsMatch(terminalLineDto, groupCondition);
            }
            else if (matchCondition is FieldCondition fieldCondition)
            {
                return IsMatch(terminalLineDto, fieldCondition);
            }
            else
            {
                return false;
            }
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, GroupCondition groupCondition)
        {
            if (groupCondition == null)
            {
                throw new ArgumentNullException(nameof(groupCondition));
            }

            var conditions = groupCondition.Conditions?.Where(condition => !(condition?.IsDisabled ?? true)) ?? new List<Condition>();

            bool groupMatched;
            if (groupCondition.IsDisabled)
            {
                groupMatched = false;
            }
            else if (!conditions.Any())
            {
                groupMatched = groupCondition.DefaultResult;
            }
            else
            {
                groupMatched = groupCondition.MatchMode switch
                {
                    GroupMatchMode.All => conditions.All(condition => IsMatch(terminalLineDto, condition)),
                    GroupMatchMode.Any => conditions.Any(condition => IsMatch(terminalLineDto, condition)),
                    _ => false
                };
            }

            return groupMatched ^ groupCondition.IsInverted;
        }

        public static bool IsMatch(TerminalLineDto terminalLineDto, FieldCondition fieldCondition)
        {
            if (fieldCondition == null)
            {
                throw new ArgumentNullException(nameof(fieldCondition));
            }

            bool fieldMatched;
            if (fieldCondition.IsDisabled)
            {
                fieldMatched = false;
            }
            else if (terminalLineDto.LineFieldDict == null || !terminalLineDto.LineFieldDict.ContainsKey(fieldCondition.FieldKey))
            {
                fieldMatched = fieldCondition.DefaultResult;
            }
            else
            {
                var jsonProperty = terminalLineDto.LineFieldDict[fieldCondition.FieldKey];
                fieldMatched = TextMatcher.IsMatch(jsonProperty.Text, fieldCondition.TargetValue, fieldCondition.MatchOperator);
            }

            return fieldMatched ^ fieldCondition.IsInverted;
        }
    }
}
