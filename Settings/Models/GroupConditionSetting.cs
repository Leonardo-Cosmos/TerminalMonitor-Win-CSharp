﻿/* 2021/8/1 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Settings.Models
{
    record GroupConditionSetting(string MatchMode, List<ConditionSetting> Conditions,
         string Id, string Name, bool IsInverted, bool DefaultResult, bool IsDisabled)
         : ConditionSetting(Id: Id, Name: Name,
             IsInverted: IsInverted, DefaultResult: DefaultResult, IsDisabled: IsDisabled);

    static class GroupConditionSettings
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

        public static GroupConditionSetting Save(GroupCondition obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new GroupConditionSetting(
                MatchMode: ModeToString(obj.MatchMode),
                Conditions: obj.Conditions?
                    .Select(condition => ConditionSettings.Save(condition)).ToList(),
                Id: obj.Id,
                Name: obj.Name,
                IsInverted: obj.IsInverted,
                DefaultResult: obj.DefaultResult,
                IsDisabled: obj.IsDisabled
                );
        }

        public static GroupCondition Load(GroupConditionSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new GroupCondition()
            {
                MatchMode = StringToMode(setting.MatchMode),
                Conditions = setting.Conditions?
                    .Select(condition => ConditionSettings.Load(condition)).ToArray(),
                Id = setting.Id,
                Name = setting.Name,
                IsInverted = setting.IsInverted,
                DefaultResult = setting.DefaultResult,
                IsDisabled = setting.IsDisabled,
            };
        }
    }
}
