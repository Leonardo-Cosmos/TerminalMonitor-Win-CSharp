/* 2021/4/20 */
using System;
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

        public DateTime DateTime { get; set; }
        
        public string PlainText { get; set; }

        /// <summary>
        /// Deserialized JSON object by hierarchy.
        /// </summary>
        public Dictionary<string, object> JsonObjectDict { get; set; }

        /// <summary>
        /// JSON properties with full path as key.
        /// </summary>
        public Dictionary<string, TerminalLineFieldDto> JsonProperties { get; set; }

    }
}
