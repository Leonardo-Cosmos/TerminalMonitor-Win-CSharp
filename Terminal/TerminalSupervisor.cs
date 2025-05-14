/* 2021/12/14 */
using System;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Terminal
{
    class TerminalSupervisor : ITerminalSupervisor
    {
        private readonly List<TerminalLineDto> terminalLineDtos = [];

        private readonly TerminalLineDtoCollection terminalLineCollection;

        public TerminalSupervisor()
        {
            terminalLineCollection = new(terminalLineDtos);
        }

        public void AddTerminalLines(IEnumerable<TerminalLineDto> terminalLineDtoCollection)
        {
            terminalLineDtos.AddRange(terminalLineDtoCollection);

            OnTerminalLinesAdded([.. terminalLineDtoCollection]);
        }

        public void RemoveTerminalLinesUntil(string terminalLineId)
        {
            var index = terminalLineDtos.FindIndex(terminalLine => terminalLine.Id == terminalLineId);

            var removedTerminalLineDtos = terminalLineDtos.GetRange(0, index + 1).ToArray();
            terminalLineDtos.RemoveRange(0, index + 1);

            OnTerminalLinesRemoved(removedTerminalLineDtos);
        }

        protected void OnTerminalLinesAdded(TerminalLineDto[] terminalLineDtos)
        {
            TerminalLineDtosEventArgs e = new()
            {
                TerminalLines = terminalLineDtos,
            };

            TerminalLinesAdded?.Invoke(this, e);
        }

        protected void OnTerminalLinesRemoved(TerminalLineDto[] terminalLineDtos)
        {
            TerminalLineDtosEventArgs e = new()
            {
                TerminalLines = terminalLineDtos,
            };

            TerminalLinesRemoved?.Invoke(this, e);
        }

        public TerminalLineDtoCollection TerminalLines => terminalLineCollection;

        public event TerminalLineDtosEventHandler? TerminalLinesAdded;

        public event TerminalLineDtosEventHandler? TerminalLinesRemoved;
    }
}
