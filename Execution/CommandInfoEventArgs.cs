/* 2025/6/6 */
using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    public class CommandInfoEventArgs
    {
        public required CommandInfo Command { get; init; }
    }

    public delegate void CommandInfoEventHandler(object sender, CommandInfoEventArgs e);
}
