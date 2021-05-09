/* 2021/4/24 */
using System.Text.RegularExpressions;

namespace TerminalMonitor.Checkers
{
    static class TextChecker
    {

        public enum CheckOperator
        {
            None,
            Equals,
            Contains,
            StartsWith,
            EndsWith,
            Matches,
        }

        /// <summary>
        /// Check if text meets condition.
        /// </summary>
        /// <param name="text">Actual text.</param>
        /// <param name="value">Target value.</param>
        /// <param name="op">Operator of checking.</param>
        /// <returns></returns>
        public static bool Check(string text, string value, CheckOperator op)
        {
            bool isPassed;
            switch (op)
            {
                case CheckOperator.Equals:
                    isPassed = text.Equals(value);
                    break;

                case CheckOperator.Contains:
                    isPassed = text.Contains(value);
                    break;

                case CheckOperator.StartsWith:
                    isPassed = text.StartsWith(value);
                    break;

                case CheckOperator.EndsWith:
                    isPassed = text.EndsWith(value);
                    break;

                case CheckOperator.Matches:
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
