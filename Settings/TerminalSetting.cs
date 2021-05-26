/* 2021/5/26 */
using System.Collections.Generic;
using TerminalMonitor.Settings.Models;

namespace TerminalMonitor.Settings
{
    class TerminalSetting
    {
        public List<FieldDisplayDetailSetting> Fields { get; set; }

        public List<FilterConditionSetting> Filters { get; set; }
    }
}
