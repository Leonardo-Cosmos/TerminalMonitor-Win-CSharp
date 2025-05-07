/* 2021/6/19 */
using System;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Models
{
    public class TerminalConfig : ICloneable
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public IEnumerable<FieldDisplayDetail>? VisibleFields { get; set; }

        public GroupCondition? FilterCondition { get; set; }

        public GroupCondition? FindCondition { get; set; }

        public object Clone()
        {
            TerminalConfig clone = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = this.Name,
                VisibleFields = this.VisibleFields?.Select(field => (FieldDisplayDetail)field.Clone()),
                FilterCondition = this.FilterCondition?.Clone() as GroupCondition,
                FindCondition = this.FindCondition?.Clone() as GroupCondition,
            };

            return clone;
        }
    }
}
