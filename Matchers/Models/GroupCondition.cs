/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerminalMonitor.Matchers.Models
{
    public class GroupCondition : Condition, ICloneable
    {
        private GroupMatchMode _matchMode;

        private List<Condition>? _conditions;

        public GroupCondition(string id, string? name, GroupMatchMode matchMode, List<Condition>? conditions) : base(id, name)
        {
            _matchMode = matchMode;
            _conditions = conditions;
        }

        public GroupCondition(string? name, GroupMatchMode matchMode, List<Condition>? conditions) : base(name)
        {
            _matchMode = matchMode;
            _conditions = conditions;
        }

        public GroupCondition(GroupMatchMode matchMode, List<Condition>? conditions) : base()
        {
            _matchMode = matchMode;
            _conditions = conditions;
        }

        protected GroupCondition(GroupCondition obj) : base(obj)
        {
            MatchMode = obj.MatchMode;
            if (obj.Conditions != null)
            {
                Conditions = [.. obj.Conditions.Select(condition => (Condition)condition.Clone())];
            }
        }

        public override object Clone()
        {
            return new GroupCondition(this);
        }

        public static GroupCondition Empty => new(
            GroupMatchMode.All,
            null
        );

        public GroupMatchMode MatchMode
        {
            get => _matchMode;
            set => _matchMode = value;
        }

        public List<Condition>? Conditions
        {
            get => _conditions;
            set => _conditions = value;
        }
    }
}
