/* 2021/6/20 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Terminal
{
    public interface ITerminalSupervisor
    {
        public void AddTerminalLines(IEnumerable<TerminalLine> terminalLineDtoCollection);

        public void RemoveTerminalLinesUntil(string terminalLineId);

        TerminalLineDtoCollection TerminalLines { get; }

        event TerminalLineDtosEventHandler? TerminalLinesAdded;

        event TerminalLineDtosEventHandler? TerminalLinesRemoved;
    }
}
