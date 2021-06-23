/* 2021/6/20 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    public class TerminalLineEventArgs : EventArgs
    {
        public TerminalLineDto TerminalLine { get; set; }
    }

    public delegate void TerminalLineEventHandler(object sender, TerminalLineEventArgs e);
}
