/* 2021/4/27 */
using System;

namespace TerminalMonitor.Matchers.Models
{
    [Obsolete]
    public class FilterCondition : ICloneable
    {
        public FieldCondition Condition { get; set; }

        public bool Excluded { get; set; }

        public object Clone()
        {
            return new FilterCondition()
            {
                Condition = (FieldCondition)this.Condition.Clone(),
                Excluded = this.Excluded,
            };
        }
    }
}
