/* 2021/5/26 */
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record FieldDisplayDetailSetting(string FieldKey, bool CustomizeStyle,
        TextStyleSetting Style, List<TextStyleConditionSetting> Conditions);

    static class FieldDisplayDetailSettings
    {
        public static FieldDisplayDetailSetting Save(FieldDisplayDetail obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new FieldDisplayDetailSetting(
                FieldKey: obj.FieldKey,
                CustomizeStyle: obj.CustomizeStyle,
                Style: TextStyleSettings.Save(obj.Style),
                Conditions: obj.Conditions?
                    .Select(condition => TextStyleConditionSettings.Save(condition)).ToList()
                );
        }

        public static FieldDisplayDetail Load(FieldDisplayDetailSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new FieldDisplayDetail()
            {
                FieldKey = setting.FieldKey,
                CustomizeStyle = setting.CustomizeStyle,
                Style = TextStyleSettings.Load(setting.Style),
                Conditions = setting.Conditions?
                    .Select(condition => TextStyleConditionSettings.Load(condition)),
            };
        }
    }
}
