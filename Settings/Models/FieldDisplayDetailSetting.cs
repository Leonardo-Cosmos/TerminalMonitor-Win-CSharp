/* 2021/5/26 */
using System;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record FieldDisplayDetailSetting(string Id, string FieldKey, bool Hidden,
        string? HeaderName, bool CustomizeHeader, ColumnHeaderStyleSetting? HeaderStyle,
        bool CustomizeStyle, TextStyleSetting? Style, List<TextStyleConditionSetting>? Conditions);

    static class FieldDisplayDetailSettings
    {
        public static FieldDisplayDetailSetting? Save(FieldDisplayDetail? obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new FieldDisplayDetailSetting(
                Id: obj.Id,
                FieldKey: obj.FieldKey,
                Hidden: obj.Hidden,
                HeaderName: obj.HeaderName,
                CustomizeHeader: obj.CustomizeHeader,
                HeaderStyle: ColumnHeaderStyleSettings.Save(obj.HeaderStyle),
                CustomizeStyle: obj.CustomizeStyle,
                Style: TextStyleSettings.Save(obj.Style),
                Conditions: obj.Conditions?
                    .Select(condition => TextStyleConditionSettings.Save(condition)!).ToList()
                );
        }

        public static FieldDisplayDetail? Load(FieldDisplayDetailSetting? setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new FieldDisplayDetail()
            {
                Id = setting.Id ?? Guid.NewGuid().ToString(),
                FieldKey = setting.FieldKey,
                Hidden = setting.Hidden,
                HeaderName = setting.HeaderName,
                CustomizeHeader = setting.CustomizeHeader,
                HeaderStyle = ColumnHeaderStyleSettings.Load(setting.HeaderStyle) ?? ColumnHeaderStyle.Empty,
                CustomizeStyle = setting.CustomizeStyle,
                Style = TextStyleSettings.Load(setting.Style) ?? TextStyle.Empty,
                Conditions = setting.Conditions?
                    .Select(condition => TextStyleConditionSettings.Load(condition)!).ToArray(),
            };
        }
    }
}
