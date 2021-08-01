/* 2021/8/1 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Settings.Models
{
    record ConditionGroupSetting(string MatchMode, List<ConditionSetting> Conditions,
         string Name, bool NegativeMatch, bool DefaultMatch, bool DismissMatch)
         : ConditionSetting(Name, NegativeMatch, DefaultMatch, DismissMatch);

    static class ConditionGroupSettings
    {
        private static IReadOnlyDictionary<string, GroupMatchMode> matchModeDict
            = InitMatchModeDict();

        private static IReadOnlyDictionary<string, GroupMatchMode> InitMatchModeDict()
        {
            Dictionary<string, GroupMatchMode> dict = new();
            var matchModes = (GroupMatchMode[])Enum.GetValues(typeof(GroupMatchMode));
            foreach (var matchMode in matchModes)
            {
                dict.Add(matchMode.ToString(), matchMode);
            }

            return new ReadOnlyDictionary<string, GroupMatchMode>(dict);
        }

        static string ModeToString(GroupMatchMode matchMode)
        {
            return matchMode.ToString();
        }

        static GroupMatchMode StringToMode(string str)
        {
            if (matchModeDict.ContainsKey(str))
            {
                return matchModeDict[str];
            }
            else
            {
                return GroupMatchMode.All;
            }
        }

        public static ConditionGroupSetting Save(ConditionGroup obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new ConditionGroupSetting(
                MatchMode: ModeToString(obj.MatchMode),
                Conditions: obj.Conditions?
                    .Select(condition => ConditionSettings.Save(condition)).ToList(),
                Name: obj.Name,
                NegativeMatch: obj.NegativeMatch,
                DefaultMatch: obj.DefaultMatch,
                DismissMatch: obj.DismissMatch
                );
        }

        public static ConditionGroup Load(ConditionGroupSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new ConditionGroup()
            {
                MatchMode = StringToMode(setting.MatchMode),
                Conditions = setting.Conditions?
                    .Select(condition => ConditionSettings.Load(condition)),
                Name = setting.Name,
                NegativeMatch = setting.NegativeMatch,
                DefaultMatch = setting.DefaultMatch,
                DismissMatch = setting.DismissMatch,
            };
        }
    }
}
