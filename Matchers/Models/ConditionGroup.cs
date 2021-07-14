/* 2021/7/9 */
using System.Collections.Generic;

namespace TerminalMonitor.Matchers.Models
{
    public class ConditionGroup : Condition
    {
        public GroupMatchMode MatchMode { get; set; }

        public IEnumerable<Condition> Conditions { get; set; }
    }
}
