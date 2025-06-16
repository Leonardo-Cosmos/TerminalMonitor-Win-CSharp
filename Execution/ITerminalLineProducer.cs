/* 2021/5/9 */
using System;
using System.Collections.Generic;
using TerminalMonitor.Models;

namespace TerminalMonitor.Execution
{
    public interface ITerminalLineProducer
    {
        IEnumerable<TerminalLine> ReadTerminalLines();

        event EventHandler? Started;

        event EventHandler? Completed;

        bool IsCompleted
        {
            get;
        }
    }
}
