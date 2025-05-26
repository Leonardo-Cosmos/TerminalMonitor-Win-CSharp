/* 2021/6/11 */
using System;
using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    public class ExecutionInfoEventArgs : EventArgs
    {
        public required ExecutionInfo Execution { get; set; }

        public Exception? Exception { get; set; }
    }

    public delegate void ExecutionInfoEventHandler(object sender, ExecutionInfoEventArgs e);
}
