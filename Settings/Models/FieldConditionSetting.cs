/* 2021/5/26 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Settings.Models
{
    record FieldConditionSetting(string FieldKey, string MatchOperator, string TargetValue);

    static class FieldConditionSettings
    {
        private static readonly IReadOnlyDictionary<string, TextMatchOperator> matchOperatorDict;

        static FieldConditionSettings()
        {
            var dict = new Dictionary<string, TextMatchOperator>();
            var matchOperators = Enum.GetValues(typeof(TextMatchOperator));
            foreach (var item in matchOperators)
            {
                if (item is TextMatchOperator matchOperator)
                {
                    dict.Add(matchOperator.ToString(), matchOperator);
                }
            }

            matchOperatorDict = new ReadOnlyDictionary<string, TextMatchOperator>(dict);
        }

        static string OperatorToString(TextMatchOperator matchOperator)
        {
            return matchOperator.ToString();
        }

        static TextMatchOperator StringToOperator(string str)
        {
            if (matchOperatorDict.ContainsKey(str))
            {
                return matchOperatorDict[str];
            }
            else
            {
                return TextMatchOperator.None;
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
