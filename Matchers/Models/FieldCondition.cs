/* 2021/4/27 */
using System;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Matchers.Models
{
    public class FieldCondition : Condition, ICloneable
    {
        public static FieldCondition Empty => new()
        {
            FieldKey = String.Empty,
            MatchOperator = TextMatcher.MatchOperator.None,
            TargetValue = String.Empty,
        };

        public string FieldKey { get; set; }

        public TextMatcher.MatchOperator MatchOperator { get; set; }

        public string TargetValue { get; set; }

        public object Clone()
        {
            return new FieldCondition()
            {
                FieldKey = this.FieldKey,
                MatchOperator = this.MatchOperator,
                TargetValue = this.TargetValue,
            };
        }
    }
}
