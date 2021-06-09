/* 2021/6/8 */
using System;

namespace TerminalMonitor.Execution
{
    class TerminalLineEventArgs : EventArgs
    {
        public string Line { get; set; }
    }

    delegate void TerminalLineEventHandler(object sender, TerminalLineEventArgs e);
}
