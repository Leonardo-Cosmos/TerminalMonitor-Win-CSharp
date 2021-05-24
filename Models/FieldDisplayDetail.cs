/* 2021/5/22 */
using System.Collections.Generic;

namespace TerminalMonitor.Models
{
    public class FieldDisplayDetail
    {
        public string FieldKey { get; set; }

        public bool UseDefaultStyle { get; set; }

        public TextStyle Style { get; set; }

        public IEnumerable<TextStyleCondition> Conditions { get; set; }
    }
}
