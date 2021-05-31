/* 2021/4/19 */
using System.Collections.Generic;
using TerminalMonitor.Settings.Models;

namespace TerminalMonitor.Settings
{
    class TerminalMonitorSetting
    {
        public List<Models.CommandSetting> Commands { get; set; }

        public List<TerminalSetting> Terminals { get; set; }
    }
}
