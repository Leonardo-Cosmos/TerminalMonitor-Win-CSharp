/* 2021/7/26 */
using System;

namespace TerminalMonitor.Execution
{
    class ProcessInfoEventArgs : EventArgs
    {
        public Exception? Exception { get; init; }
    }

    delegate void ProcessInfoEventHandler(object sender, ProcessInfoEventArgs e);
}
