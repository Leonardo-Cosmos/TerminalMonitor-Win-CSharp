/* 2021/5/21 */
using System;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Models
{
    public class TextStyleCondition : ICloneable
    {
        public TextStyle? Style { get; set; }

        public FieldCondition? Condition { get; set; }

        public object Clone()
        {
            return new TextStyleCondition()
            {
                Style = this.Style?.Clone() as TextStyle,
                Condition = this.Condition?.Clone() as FieldCondition,
            };
        }
    }
}
