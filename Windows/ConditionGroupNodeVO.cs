/* 2021/7/12 */
using System;
using System.Collections.ObjectModel;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Windows
{
    class ConditionGroupNodeVO : ConditionNodeVO
    {
        private static readonly Array matchModes = Enum.GetValues(typeof(GroupMatchMode));

        public static Array MatchModes => matchModes;

        private GroupMatchMode matchMode;

        public GroupMatchMode MatchMode
        {
            get => matchMode;
            set
            {
                matchMode = value;
                OnPropertyChanged();
            }
        }

        private readonly ObservableCollection<ConditionNodeVO> conditions = new ObservableCollection<ConditionNodeVO>();

        public ObservableCollection<ConditionNodeVO> Conditions
        {
            get => conditions;
        }
    }
}
