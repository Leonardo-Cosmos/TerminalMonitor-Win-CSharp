/* 2021/4/20 */
using System.Collections.Generic;

namespace TerminalMonitor.Windows
{
    class TerminalListItem
    {

        public string PlainText { get; set; }

        public List<TerminalListItemField> ParsedFields { get; set; }

    }
}
