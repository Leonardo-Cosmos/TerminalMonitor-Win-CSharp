/* 2021/4/27 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Checkers;

namespace TerminalMonitor.Models
{
    class TextCondition
    {

        public string FieldKey { get; set; }

        public TextChecker.CheckOperator CheckOperator { get; set; }

        public string TargetValue { get; set; }

    }
}
