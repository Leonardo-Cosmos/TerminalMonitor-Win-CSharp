/* 2021/6/19 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalMonitor.Models
{
    public class TerminalConfig : ICloneable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<FieldDisplayDetail> VisibleFields { get; set; }

        public IEnumerable<FilterCondition> FilterConditions { get; set; }

        public object Clone()
        {
            TerminalConfig clone = new()
            {
                Id = this.Id,
                Name = this.Name,
                VisibleFields = this.VisibleFields?.Select(field => (FieldDisplayDetail)field.Clone()),
                FilterConditions = this.FilterConditions?.Select(filter => (FilterCondition)filter.Clone()),
            };

            return clone;
        }
    }
}
