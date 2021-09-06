/* 2021/8/1 */
using System;
using System.Text.Json.Serialization;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Settings.Models
{
    [JsonConverter(typeof(ConditionSettingConverter))]
    abstract record ConditionSetting(string Id, string Name, bool IsInverted, bool DefaultResult, bool IsDisabled)
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
            else if (obj is GroupCondition groupCondition)
            {
                setting = GroupConditionSettings.Save(groupCondition);
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
            else if (setting is GroupConditionSetting groupConditionSetting)
            {
                condition = GroupConditionSettings.Load(groupConditionSetting);
            }
            else
            {
                throw new NotImplementedException("Unknown condition setting type");
            }
            return condition;
        }
    }
}
