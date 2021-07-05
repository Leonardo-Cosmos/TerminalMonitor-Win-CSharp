/* 2021/4/27 */
using System;

namespace TerminalMonitor.Models
{
    public class FilterCondition : ICloneable
    {
        public TextCondition Condition { get; set; }

        public bool Excluded { get; set; }

        public object Clone()
        {
            return new FilterCondition()
            {
                Condition = (TextCondition)this.Condition.Clone(),
                Excluded = this.Excluded,
            };
        }
    }
}
