/* 2021/5/22 */
using System.Collections.Generic;

namespace TerminalMonitor.Models
{
    class FieldStyleCondition
    {
        public string FieldKey { get; set; }

        public IEnumerable<TextStyleCondition> Conditions { get; set; }

        public TextStyle DefaultStyle { get; set; }
    }
}
