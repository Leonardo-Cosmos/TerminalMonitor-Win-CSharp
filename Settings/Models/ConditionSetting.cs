/* 2021/8/1 */
using System;
using System.Text.Json.Serialization;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Settings.Models
{
    [JsonConverter(typeof(ConditionSettingConverter))]
    abstract record ConditionSetting(string Name, bool NegativeMatch, bool DefaultMatch, bool DismissMatch)
    {
        public string ConditionType { get; set; }
    }

    static class ConditionSettings
    {
        public static ConditionSetting Save(Condition obj)
        {
            if (obj == null)
            {
                return null;
            }

            ConditionSetting setting;
            if (obj is FieldCondition fieldCondition)
            {
                setting = FieldConditionSettings.Save(fieldCondition);
            }
            else if (obj is ConditionGroup conditionGroup)
            {
                setting = ConditionGroupSettings.Save(conditionGroup);
            }
            else
            {
                throw new NotImplementedException("Unknown condition type");
            }
            return setting;
        }

        public static Condition Load(ConditionSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            Condition condition;
            if (setting is FieldConditionSetting fieldConditionSetting)
            {
                condition = FieldConditionSettings.Load(fieldConditionSetting);
            }
            else if (setting is ConditionGroupSetting conditionGroupSetting)
            {
                condition = ConditionGroupSettings.Load(conditionGroupSetting);
            }
            else
            {
                throw new NotImplementedException("Unknown condition setting type");
            }
            return condition;
        }
    }
}
