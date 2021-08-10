/* 2021/4/24 */
using System.Text.RegularExpressions;

namespace TerminalMonitor.Matchers
{
    public static class TextMatcher
    {
        /// <summary>
        /// Indicates whether the text meets specified condition.
        /// </summary>
        /// <param name="text">Actual text.</param>
        /// <param name="value">Target value.</param>
        /// <param name="op">Operator of matching.</param>
        /// <returns></returns>
        public static bool IsMatch(string text, string value, TextMatchOperator op)
        {
            bool isPassed;
            switch (op)
            {
                case TextMatchOperator.Equals:
                    isPassed = text.Equals(value);
                    break;

                case TextMatchOperator.Contains:
                    isPassed = text.Contains(value);
                    break;

                case TextMatchOperator.StartsWith:
                    isPassed = text.StartsWith(value);
                    break;

                case TextMatchOperator.EndsWith:
                    isPassed = text.EndsWith(value);
                    break;

                case TextMatchOperator.Matches:
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
