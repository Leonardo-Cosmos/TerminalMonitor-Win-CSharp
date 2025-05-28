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
    class TerminalLineMatcher(Condition matchCondition)
    {
        private readonly Condition matchCondition = matchCondition;

        public bool IsMatch(TerminalLine terminalLineDto)
        {
            return IsMatch(terminalLineDto, matchCondition);
        }

        public static bool IsMatch(TerminalLine terminalLineDto, Condition matchCondition)
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

        public static bool IsMatch(TerminalLine terminalLineDto, GroupCondition groupCondition)
        {
            ArgumentNullException.ThrowIfNull(groupCondition);

            var conditions = groupCondition.Conditions?.Where(condition => !(condition?.IsDisabled ?? true)) ?? [];

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

        public static bool IsMatch(TerminalLine terminalLineDto, FieldCondition fieldCondition)
        {
            ArgumentNullException.ThrowIfNull(fieldCondition);

            bool fieldMatched;
            if (fieldCondition.IsDisabled)
            {
                fieldMatched = false;
            }
            else if (!terminalLineDto.LineFieldDict.TryGetValue(fieldCondition.FieldKey, out TerminalLineField? jsonProperty))
            {
                fieldMatched = fieldCondition.DefaultResult;
            }
            else
            {
                fieldMatched = TextMatcher.IsMatch(jsonProperty.Text, fieldCondition.TargetValue, fieldCondition.MatchOperator);
            }

            return fieldMatched ^ fieldCondition.IsInverted;
        }
    }
}
