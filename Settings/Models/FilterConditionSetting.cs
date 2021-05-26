/* 2021/5/26 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Settings.Models
{
    record FilterConditionSetting(FieldConditionSetting Condition, bool Excluded);

    static class FilterConditionSettings
    {
        public static FilterConditionSetting Save(FilterCondition obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new FilterConditionSetting(
                Condition: FieldConditionSettings.Save(obj.Condition),
                Excluded: obj.Excluded
                );
        }

        public static FilterCondition Load(FilterConditionSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new FilterCondition()
            {
                Condition = FieldConditionSettings.Load(setting.Condition),
                Excluded = setting.Excluded,
            };
        }
    }
}
