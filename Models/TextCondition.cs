/* 2021/4/27 */
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
