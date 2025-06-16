/* 2021/12/14 */
using System;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Terminal
{
    class TerminalSupervisor : ITerminalSupervisor
    {
        private readonly List<TerminalLine> terminalLines = [];

        private readonly TerminalLineCollection terminalLineCollection;

        public TerminalSupervisor()
        {
            terminalLineCollection = new(terminalLines);
        }

        public void AddTerminalLines(IEnumerable<TerminalLine> terminalLineCollection)
        {
            terminalLines.AddRange(terminalLineCollection);

            OnTerminalLinesAdded([.. terminalLineCollection]);
        }

        public void RemoveTerminalLinesUntil(string terminalLineId)
        {
            var index = terminalLines.FindIndex(terminalLine => terminalLine.Id == terminalLineId);

            var removedTerminalLines = terminalLines.GetRange(0, index + 1).ToArray();
            terminalLines.RemoveRange(0, index + 1);

            OnTerminalLinesRemoved(removedTerminalLines);
        }

        protected void OnTerminalLinesAdded(TerminalLine[] terminalLines)
        {
            TerminalLinesEventArgs e = new()
            {
                TerminalLines = terminalLines,
            };

            TerminalLinesAdded?.Invoke(this, e);
        }

        protected void OnTerminalLinesRemoved(TerminalLine[] terminalLines)
        {
            TerminalLinesEventArgs e = new()
            {
                TerminalLines = terminalLines,
            };

            TerminalLinesRemoved?.Invoke(this, e);
        }

        public TerminalLineCollection TerminalLines => terminalLineCollection;

        public event TerminalLinesEventHandler? TerminalLinesAdded;

        public event TerminalLinesEventHandler? TerminalLinesRemoved;
    }
}
