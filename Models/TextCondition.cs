/* 2021/4/27 */
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Models
{
    class TextCondition
    {

        public string FieldKey { get; set; }

        public TextMatcher.MatchOperator MatchOperator { get; set; }

        public string TargetValue { get; set; }

    }
}
