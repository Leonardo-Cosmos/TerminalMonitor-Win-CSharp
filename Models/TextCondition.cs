/* 2021/4/27 */
using System;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Models
{
    public class TextCondition
    {
        public string FieldKey { get; set; }

        public TextMatcher.MatchOperator MatchOperator { get; set; }

        public string TargetValue { get; set; }

    }
}
