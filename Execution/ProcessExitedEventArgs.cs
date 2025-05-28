/* 2021/7/26 */
using System;

namespace TerminalMonitor.Execution
{
    class ProcessExitedEventArgs : EventArgs
    {
        public Exception? Exception { get; init; }
    }

    delegate void ProcessExitedEventHandler(object sender, ProcessExitedEventArgs e);
}
