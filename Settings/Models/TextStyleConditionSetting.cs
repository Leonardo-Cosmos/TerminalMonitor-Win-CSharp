/* 2021/5/26 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record TextStyleConditionSetting(TextStyleSetting Style, FieldConditionSetting Condition);

    static class TextStyleConditionSettings
    {
        public static TextStyleConditionSetting Save(TextStyleCondition obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new TextStyleConditionSetting(
                Style: TextStyleSettings.Save(obj.Style),
                Condition: FieldConditionSettings.Save(obj.Condition)
                );
        }

        public static TextStyleCondition Load(TextStyleConditionSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new TextStyleCondition()
            {
                Style = TextStyleSettings.Load(setting.Style),
                Condition = FieldConditionSettings.Load(setting.Condition),
            };
        }
    }
}
