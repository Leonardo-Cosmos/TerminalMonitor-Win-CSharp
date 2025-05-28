/* 2022/2/16 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Terminal
{
    public class TerminalLineDtoCollection : IReadOnlyList<TerminalLine>
    {
        private readonly IReadOnlyList<TerminalLine> terminalLines;

        public TerminalLineDtoCollection(IEnumerable<TerminalLine> terminalLines)
        {
            if (terminalLines is IReadOnlyList<TerminalLine> terminalLineList)
            {
                this.terminalLines = terminalLineList;
            }
            else
            {
                this.terminalLines = terminalLines.ToList().AsReadOnly();
            }
        }

        public TerminalLine? this[string id] =>
            terminalLines.FirstOrDefault(terminalLine => terminalLine.Id == id!);

        public TerminalLine this[int index] => terminalLines[index];

        public int Count => terminalLines.Count;

        public IEnumerator<TerminalLine> GetEnumerator()
        {
            return terminalLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
