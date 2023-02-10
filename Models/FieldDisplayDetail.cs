/* 2021/5/22 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalMonitor.Models
{
    public class FieldDisplayDetail : ICloneable
    {
        public string Id { get; init; }

        public string FieldKey { get; set; }

        public bool Hidden { get; set; }

        public bool CustomizeHeader { get; set; }

        public string HeaderName { get; set; }

        public ColumnHeaderStyle HeaderStyle { get; set; }

        public bool CustomizeStyle { get; set; }

        public TextStyle Style { get; set; }

        public IEnumerable<TextStyleCondition> Conditions { get; set; }

        public object Clone()
        {
            return new FieldDisplayDetail()
            {
                Id = Guid.NewGuid().ToString(),
                FieldKey = this.FieldKey,
                Hidden = this.Hidden,
                CustomizeHeader = this.CustomizeHeader,
                HeaderName = this.HeaderName,
                HeaderStyle = this.HeaderStyle,
                CustomizeStyle = this.CustomizeStyle,
                Style = (TextStyle)this.Style.Clone(),
                Conditions = this.Conditions.Select(condition => (TextStyleCondition)condition.Clone()),
            };
        }
    }
}
