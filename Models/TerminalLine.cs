﻿/* 2021/4/20 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Models
{
    public class TerminalLine
    {
        public required string Id { get; init; }

        public required DateTime Timestamp { get; init; }
        
        public required string PlainText { get; init; }

        /// <summary>
        /// Deserialized JSON object by hierarchy.
        /// </summary>
        public required Dictionary<string, object> JsonObjectDict { get; set; }

        /// <summary>
        /// A dictionary of line fields with full path as the key.
        /// </summary>
        public required Dictionary<string, TerminalLineField> LineFieldDict { get; set; }
    }
}
