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
    public class TerminalLineDtoCollection : IReadOnlyList<TerminalLineDto>
    {
        private readonly IReadOnlyList<TerminalLineDto> terminalLines;

        public TerminalLineDtoCollection(IEnumerable<TerminalLineDto> terminalLines)
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
