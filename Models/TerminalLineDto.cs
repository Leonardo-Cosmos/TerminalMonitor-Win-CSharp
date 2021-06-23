/* 2021/4/20 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Models
{
    public class TerminalLineDto
    {
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }
        
        public string PlainText { get; set; }

        /// <summary>
        /// Deserialized JSON object by hierarchy.
        /// </summary>
        public Dictionary<string, object> JsonObjectDict { get; set; }

        /// <summary>
        /// Line fields with full path as key.
        /// </summary>
        public Dictionary<string, TerminalLineFieldDto> LineFieldDict { get; set; }

    }
}
