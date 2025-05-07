/* 2021/5/22 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalMonitor.Models
{
    public class FieldDisplayDetail : ICloneable
    {
        public required string Id { get; init; }

        public required string FieldKey { get; set; }

        public bool Hidden { get; set; }

        public string? HeaderName { get; set; }

        public bool CustomizeHeader { get; set; }

        public ColumnHeaderStyle? HeaderStyle { get; set; }

        public bool CustomizeStyle { get; set; }

        public TextStyle? Style { get; set; }

        public IEnumerable<TextStyleCondition>? Conditions { get; set; }

        public object Clone()
        {
            return new FieldDisplayDetail()
            {
                Id = Guid.NewGuid().ToString(),
                FieldKey = this.FieldKey,
                Hidden = this.Hidden,
                HeaderName = this.HeaderName,
                CustomizeHeader = this.CustomizeHeader,
                HeaderStyle = this.HeaderStyle?.Clone() as ColumnHeaderStyle,
                CustomizeStyle = this.CustomizeStyle,
                Style = this.Style?.Clone() as TextStyle,
                Conditions = this.Conditions?.Select(condition => (TextStyleCondition)condition.Clone()),
            };
        }
    }
}
