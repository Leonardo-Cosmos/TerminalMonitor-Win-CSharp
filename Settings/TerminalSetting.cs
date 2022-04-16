/* 2021/5/26 */
using System;
using System.Collections.Generic;
using TerminalMonitor.Settings.Models;

namespace TerminalMonitor.Settings
{
    [Obsolete]
    class TerminalSetting
    {
        public List<FieldDisplayDetailSetting> Fields { get; set; }

        public List<FilterConditionSetting> Filters { get; set; }
    }
}
