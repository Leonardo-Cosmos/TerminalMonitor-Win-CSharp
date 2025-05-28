/* 2021/6/8 */
using System;

namespace TerminalMonitor.Execution
{
    class ProcessOutputEventArgs : EventArgs
    {
        public required string Text { get; init; }
    }

    delegate void ProcessOutputEventHandler(object sender, ProcessOutputEventArgs e);
}
