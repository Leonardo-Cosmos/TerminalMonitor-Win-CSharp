/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalMonitor.Matchers.Models
{
    public class ConditionGroup : Condition, ICloneable
    {
        public ConditionGroup()
        {

        }

        protected ConditionGroup(ConditionGroup obj) : base(obj)
        {
            MatchMode = obj.MatchMode;
            if (obj.Conditions != null)
            {
                Conditions = obj.Conditions.Select(condition => (Condition)condition.Clone());
            }
        }

        public override object Clone()
        {
            return new ConditionGroup(this);
        }

        public GroupMatchMode MatchMode { get; set; }

        public IEnumerable<Condition> Conditions { get; set; }
    }
}
