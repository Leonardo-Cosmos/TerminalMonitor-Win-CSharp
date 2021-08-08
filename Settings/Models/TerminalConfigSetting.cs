/* 2021/6/25 */
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record TerminalConfigSetting(string Id, string Name, List<FieldDisplayDetailSetting> VisibleFields, GroupConditionSetting FilterCondition);

    static class TerminalConfigSettings
    {
        public static TerminalConfigSetting Save(TerminalConfig obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new TerminalConfigSetting(
                    Id: obj.Id,
                    Name: obj.Name,
                    VisibleFields: obj.VisibleFields?
                        .Select(field => FieldDisplayDetailSettings.Save(field)).ToList(),
                    FilterCondition: GroupConditionSettings.Save(obj.FilterCondition)
                );
        }

        public static TerminalConfig Load(TerminalConfigSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new TerminalConfig()
            {
                Id = setting.Id,
                Name = setting.Name,
                VisibleFields = setting.VisibleFields?
                    .Select(field => FieldDisplayDetailSettings.Load(field)),
                FilterCondition = GroupConditionSettings.Load(setting.FilterCondition),
            };
        }
    }
}
