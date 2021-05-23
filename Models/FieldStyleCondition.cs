/* 2021/5/22 */
using System.Collections.Generic;

namespace TerminalMonitor.Models
{
    class FieldStyleCondition
    {
        public string FieldKey { get; set; }

        public IEnumerable<TextStyleCondition> Condtions { get; set; }

        public TextStyle DefaultStyle { get; set; }
    }
}
