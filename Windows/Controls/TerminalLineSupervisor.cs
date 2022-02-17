/* 2021/12/14 */
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    class TerminalLineSupervisor : ITerminalLineSupervisor
    {
        private readonly List<TerminalLineDto> terminalLineDtos = new();

        private readonly TerminalLineCollection terminalLineCollection;

        public TerminalLineSupervisor()
        {
            terminalLineCollection = new(terminalLineDtos);
        }

        public void AddTerminalLines(IEnumerable<TerminalLineDto> terminalLineDtoCollection)
        {
            terminalLineDtos.AddRange(terminalLineDtoCollection);

            OnTerminalLineAdded(terminalLineDtoCollection.ToArray());
        }

        protected void OnTerminalLineAdded(TerminalLineDto[] terminalLineDtos)
        {
            TerminalLinesEventArgs e = new()
            {
                TerminalLines = terminalLineDtos,
            };

            TerminalLinesAdded?.Invoke(this, e);
        }

        public TerminalLineCollection TerminalLines => terminalLineCollection;


        public event TerminalLinesEventHandler TerminalLinesAdded;
    }
}
