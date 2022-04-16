/* 2021/4/19 */
using System.Collections.Generic;
using TerminalMonitor.Settings.Models;

namespace TerminalMonitor.Settings
{
    class TerminalMonitorSetting
    {
        public List<CommandConfigSetting> Commands { get; set; }

        public List<TerminalConfigSetting> Terminals { get; set; }
    }
}
