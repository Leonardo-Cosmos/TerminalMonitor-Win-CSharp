/* 2021/4/27 */
using System;

namespace TerminalMonitor.Matchers.Models
{
    [Obsolete]
    public class FilterCondition : ICloneable
    {
        public FieldCondition? Condition { get; set; }

        public bool Excluded { get; set; }

        public object Clone()
        {
            return new FilterCondition()
            {
                Condition = this.Condition?.Clone() as FieldCondition,
                Excluded = this.Excluded,
            };
        }
    }
}
