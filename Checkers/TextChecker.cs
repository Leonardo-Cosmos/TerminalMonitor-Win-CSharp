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
