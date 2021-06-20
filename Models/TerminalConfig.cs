/* 2021/6/19 */
using System.Collections.Generic;

namespace TerminalMonitor.Models
{
    public class TerminalConfig
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<FieldDisplayDetail> VisibleFields { get; set; }

        public IEnumerable<FilterCondition> FilterConditions { get; set; }
    }
}
