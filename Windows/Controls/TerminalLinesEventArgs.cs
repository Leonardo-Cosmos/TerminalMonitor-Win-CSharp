/* 2021/6/20 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    public class TerminalLinesEventArgs : EventArgs
    {
        public IEnumerable<TerminalLineDto> TerminalLines { get; set; }
    }

    public delegate void TerminalLinesEventHandler(object sender, TerminalLinesEventArgs e);
}
