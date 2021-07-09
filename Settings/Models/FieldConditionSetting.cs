/* 2021/5/26 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TerminalMonitor.Matchers.Models;
using static TerminalMonitor.Matchers.TextMatcher;

namespace TerminalMonitor.Settings.Models
{
    record FieldConditionSetting(string FieldKey, string MatchOperator, string TargetValue);

    static class FieldConditionSettings
    {
        private static readonly IReadOnlyDictionary<string, MatchOperator> matchOperatorDict;

        static FieldConditionSettings()
        {
            var dict = new Dictionary<string, MatchOperator>();
            var matchOperators = Enum.GetValues(typeof(MatchOperator));
            foreach (var item in matchOperators)
            {
                if (item is MatchOperator matchOperator)
                {
                    dict.Add(matchOperator.ToString(), matchOperator);
                }
            }

            matchOperatorDict = new ReadOnlyDictionary<string, MatchOperator>(dict);
        }

        static string OperatorToString(MatchOperator matchOperator)
        {
            return matchOperator.ToString();
        }

        static MatchOperator StringToOperator(string str)
        {
            if (matchOperatorDict.ContainsKey(str))
            {
                return matchOperatorDict[str];
            }
            else
            {
                return MatchOperator.None;
            }
        }

        public static FieldConditionSetting Save(FieldCondition obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new FieldConditionSetting(
                FieldKey: obj.FieldKey,
                MatchOperator: OperatorToString(obj.MatchOperator),
                TargetValue: obj.TargetValue
                );
        }

        public static FieldCondition Load(FieldConditionSetting setting)
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
            };
        }
    }
}
