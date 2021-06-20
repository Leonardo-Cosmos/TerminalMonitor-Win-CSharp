/* 2021/6/20 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Windows.Controls
{
    public interface ITerminalLineSupervisor
    {
        IEnumerable<TerminalLineDto> TerminalLines { get; }

        event TerminalLineEventHandler TerminalLineAdded;
    }
}
