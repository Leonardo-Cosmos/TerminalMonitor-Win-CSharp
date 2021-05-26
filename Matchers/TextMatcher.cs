/* 2021/4/24 */
using System.Text.RegularExpressions;

namespace TerminalMonitor.Matchers
{
    public static class TextMatcher
    {

        public enum MatchOperator
        {
            None,
            Equals,
            Contains,
            StartsWith,
            EndsWith,
            Matches,
        }

        /// <summary>
        /// Indicates whether the text meets specified condition.
        /// </summary>
        /// <param name="text">Actual text.</param>
        /// <param name="value">Target value.</param>
        /// <param name="op">Operator of matching.</param>
        /// <returns></returns>
        public static bool IsMatch(string text, string value, MatchOperator op)
        {
            bool isPassed;
            switch (op)
            {
                case MatchOperator.Equals:
                    isPassed = text.Equals(value);
                    break;

                case MatchOperator.Contains:
                    isPassed = text.Contains(value);
                    break;

                case MatchOperator.StartsWith:
                    isPassed = text.StartsWith(value);
                    break;

                case MatchOperator.EndsWith:
                    isPassed = text.EndsWith(value);
                    break;

                case MatchOperator.Matches:
                    Regex regex = new(value);
                    isPassed = regex.IsMatch(text);
                    break;

                default:
                    isPassed = false;
                    break;
            }

            return isPassed;
        }

    }
}
