/* 2021/6/20 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    public interface ITerminalLineSupervisor
    {
        TerminalLineCollection TerminalLines { get; }

        event TerminalLineEventHandler TerminalLineAdded;
    }

    public class TerminalLineCollection : IEnumerable<TerminalLineDto>
    {
        private readonly IEnumerable<TerminalLineDto> terminalLines;

        public TerminalLineCollection(IEnumerable<TerminalLineDto> terminalLines)
        {
            this.terminalLines = terminalLines!;
        }

        public TerminalLineDto this[string id] =>
            terminalLines.FirstOrDefault(terminalLine => terminalLine.Id == id!);

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
