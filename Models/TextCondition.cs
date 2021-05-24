/* 2021/4/27 */
using System;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Models
{
    public class TextCondition
    {
        private static readonly TextCondition defaultInstance = new()
        {
            FieldKey = String.Empty,
            MatchOperator = TextMatcher.MatchOperator.None,
            TargetValue = String.Empty,
        };

        public static TextCondition Default => defaultInstance;

        public string FieldKey { get; set; }

        public TextMatcher.MatchOperator MatchOperator { get; set; }

        public string TargetValue { get; set; }

    }
}
