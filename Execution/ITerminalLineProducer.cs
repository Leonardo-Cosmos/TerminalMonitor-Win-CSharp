/* 2021/5/9 */
using System;
using System.Collections.Generic;

namespace TerminalMonitor.Execution
{
    public interface ITerminalLineProducer
    {
        IEnumerable<string> ReadTerminalLines();

        event EventHandler Started;

        event EventHandler Completed;

        bool IsCompleted
        {
            get;
        }
    }
}
