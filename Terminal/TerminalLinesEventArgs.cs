/* 2021/6/20 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Terminal
{
    public class TerminalLinesEventArgs : EventArgs
    {
        public required IEnumerable<TerminalLine> TerminalLines { get; set; }
    }

    public delegate void TerminalLinesEventHandler(object sender, TerminalLinesEventArgs e);
}
