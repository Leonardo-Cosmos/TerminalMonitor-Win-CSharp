/* 2021/6/20 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    public interface ITerminalLineSupervisor
    {
        TerminalLineCollection TerminalLines { get; }

        event TerminalLinesEventHandler TerminalLinesAdded;
    }

    public class TerminalLineCollection : IReadOnlyList<TerminalLineDto>
    {
        private readonly IReadOnlyList<TerminalLineDto> terminalLines;

        public TerminalLineCollection(IEnumerable<TerminalLineDto> terminalLines)
        {
            if (terminalLines is IReadOnlyList<TerminalLineDto> terminalLineList)
            {
                this.terminalLines = terminalLineList;
            }
            else
            {
                this.terminalLines = terminalLines.ToList();
            }
        }

        public TerminalLineDto this[string id] =>
            terminalLines.FirstOrDefault(terminalLine => terminalLine.Id == id!);

        public TerminalLineDto this[int index] => terminalLines[index];

        public int Count => terminalLines.Count;

        public IEnumerator<TerminalLineDto> GetEnumerator()
        {
            return terminalLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
