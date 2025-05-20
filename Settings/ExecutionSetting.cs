/* 2021/4/19 */

using System;

namespace TerminalMonitor.Settings
{
    [Obsolete]
    class ExecutionSetting
    {
        public string? CommandName { get; set; }

        public string? ArgumentsText { get; set; }

        public string? WorkingDirectory { get; set; }
    }
}
