/* 2021/4/20 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    public class TerminalLineDto
    {
        public string Id { get; set; }
        
        public string PlainText { get; set; }

        public Dictionary<string, object> ParsedFieldDict { get; set; }

        public List<TerminalLineFieldVO> ParsedFields { get; set; }

    }
}
