/* 2021/5/21 */
using System;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Models
{
    public class TextStyleCondition : ICloneable
    {
        public TextStyle Style { get; set; }

        public FieldCondition Condition { get; set; }

        public object Clone()
        {
            return new TextStyleCondition()
            {
                Style = (TextStyle)this.Style.Clone(),
                Condition = (FieldCondition)this.Condition.Clone(),
            };
        }
    }
}
