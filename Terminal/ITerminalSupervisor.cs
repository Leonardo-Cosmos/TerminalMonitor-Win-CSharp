/* 2021/6/20 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Terminal
{
    public interface ITerminalSupervisor
    {
        public void AddTerminalLines(IEnumerable<TerminalLine> terminalLineCollection);

        public void RemoveTerminalLinesUntil(string terminalLineId);

        TerminalLineCollection TerminalLines { get; }

        event TerminalLinesEventHandler? TerminalLinesAdded;

        event TerminalLinesEventHandler? TerminalLinesRemoved;
    }
}
