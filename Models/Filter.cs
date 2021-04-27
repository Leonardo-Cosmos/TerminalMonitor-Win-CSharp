/* 2021/4/27 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Models
{
    class Filter
    {
        public TextCondition Condition { get; set; }

        public bool Excluded { get; set; }

    }
}
