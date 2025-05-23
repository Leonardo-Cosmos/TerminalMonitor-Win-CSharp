﻿/* 2021/6/8 */
using System;

namespace TerminalMonitor.Execution
{
    class TerminalLineEventArgs : EventArgs
    {
        public required string Line { get; init; }
    }

    delegate void TerminalLineEventHandler(object sender, TerminalLineEventArgs e);
}
