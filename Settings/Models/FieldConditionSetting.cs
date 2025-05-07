/* 2021/5/26 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TerminalMonitor.Matchers;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Settings.Models
{
    record FieldConditionSetting(string FieldKey, string MatchOperator, string TargetValue,
        string Id, string Name, bool IsInverted, bool DefaultResult, bool IsDisabled)
        : ConditionSetting(Id: Id, Name: Name,
            IsInverted: IsInverted, DefaultResult: DefaultResult, IsDisabled: IsDisabled);

    static class FieldConditionSettings
    {
        private static readonly ReadOnlyDictionary<string, TextMatchOperator> matchOperatorDict
            = InitMatchOperatorDict();

        private static ReadOnlyDictionary<string, TextMatchOperator> InitMatchOperatorDict()
        {
            Dictionary<string, TextMatchOperator> dict = [];
            var matchOperators = Enum.GetValues(typeof(TextMatchOperator));
            foreach (var item in matchOperators)
            {
                if (item is TextMatchOperator matchOperator)
                {
                    dict.Add(matchOperator.ToString(), matchOperator);
                }
            }

            return new ReadOnlyDictionary<string, TextMatchOperator>(dict);
        }

        static string OperatorToString(TextMatchOperator matchOperator)
        {
            return matchOperator.ToString();
        }

        static TextMatchOperator StringToOperator(string str)
        {
            if (str != null && matchOperatorDict.TryGetValue(str, out TextMatchOperator value))
            {
                return value;
            }
            else
            {
                return TextMatchOperator.None;
            }
        }

        public static FieldConditionSetting? Save(FieldCondition? obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new FieldConditionSetting(
                FieldKey: obj.FieldKey,
                MatchOperator: OperatorToString(obj.MatchOperator),
                TargetValue: obj.TargetValue,
                Id: obj.Id,
                Name: obj.Name,
                IsInverted: obj.IsInverted,
                DefaultResult: obj.DefaultResult,
                IsDisabled: obj.IsDisabled
                );
        }

        public static FieldCondition? Load(FieldConditionSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new FieldCondition()
            {
                FieldKey = setting.FieldKey,
                MatchOperator = StringToOperator(setting.MatchOperator),
                TargetValue = setting.TargetValue,
                Id = setting.Id ?? Guid.NewGuid().ToString(),
                Name = setting.Name,
                IsInverted = setting.IsInverted,
                DefaultResult = setting.DefaultResult,
                IsDisabled = setting.IsDisabled,
            };
        }
    }
}
