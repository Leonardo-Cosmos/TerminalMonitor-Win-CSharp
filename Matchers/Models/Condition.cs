/* 2021/7/9 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Matchers.Models
{
    public abstract class Condition
    {
        public bool Negative { get; set; }

        public bool Default { get; set; }

        public bool Disabled { get; set; }
    }
}
