/* 2021/6/9 */
using System;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    public class CommandRunEventArgs : EventArgs
    {
        public required CommandConfig Command { get; set; }
    }

    public delegate void CommandRunEventHandler(object sender, CommandRunEventArgs e);
}
