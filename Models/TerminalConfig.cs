/* 2021/6/19 */
using System;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Models
{
    public class TerminalConfig : ICloneable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<FieldDisplayDetail> VisibleFields { get; set; }

        public ConditionGroup FilterCondition { get; set; }

        public object Clone()
        {
            TerminalConfig clone = new()
            {
                Id = this.Id,
                Name = this.Name,
                VisibleFields = this.VisibleFields?.Select(field => (FieldDisplayDetail)field.Clone()),
                FilterCondition = this.FilterCondition?.Clone() as ConditionGroup,
            };

            return clone;
        }
    }
}
