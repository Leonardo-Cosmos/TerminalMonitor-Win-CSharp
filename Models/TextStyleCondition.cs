/* 2021/5/21 */
using System;

namespace TerminalMonitor.Models
{
    public class TextStyleCondition : ICloneable
    {
        public TextStyle Style { get; set; }

        public TextCondition Condition { get; set; }

        public object Clone()
        {
            return new TextStyleCondition()
            {
                Style = (TextStyle)this.Style.Clone(),
                Condition = (TextCondition)this.Condition.Clone(),
            };
        }
    }
}
