/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Matchers.Models
{
    public class ConditionGroup : Condition
    {
        public bool MatchAny { get; set; }

        public IEnumerable<Condition> Conditions { get; set; }
    }
}
