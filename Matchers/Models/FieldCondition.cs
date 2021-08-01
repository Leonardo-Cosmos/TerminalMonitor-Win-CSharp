/* 2021/4/27 */
using System;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Matchers.Models
{
    public class FieldCondition : Condition, ICloneable
    {
        public FieldCondition()
        {

        }

        protected FieldCondition(FieldCondition obj) : base(obj)
        {
            FieldKey = obj.FieldKey;
            MatchOperator = obj.MatchOperator;
            TargetValue = obj.TargetValue;
        }

        public override object Clone()
        {
            return new FieldCondition(this);
        }

        public static FieldCondition Empty => new()
        {
            FieldKey = String.Empty,
            MatchOperator = TextMatchOperator.None,
            TargetValue = String.Empty,
        };

        public string FieldKey { get; set; }

        public TextMatchOperator MatchOperator { get; set; }

        public string TargetValue { get; set; }
    }
}
