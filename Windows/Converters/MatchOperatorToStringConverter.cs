/* 2021/4/24 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TerminalMonitor.Matchers;

namespace TerminalMonitor.Windows.Converters
{
    class MatchOperatorToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<TextMatchOperator, string> opTextDict =
            new(new Dictionary<TextMatchOperator, string>() {
                { TextMatchOperator.None, "--" },
                { TextMatchOperator.Equals, "equals"},
                { TextMatchOperator.Contains, "contains" },
                { TextMatchOperator.StartsWith, "starts with" },
                { TextMatchOperator.EndsWith, "ends with" },
                { TextMatchOperator.Matches, "matches" },
            });

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "?";
            }

            if (value is TextMatchOperator matchOperator)
            {
                return opTextDict[matchOperator];
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return TextMatchOperator.None;
            }

            if (value is string text)
            {
                return opTextDict
                    .FirstOrDefault(kvPair => kvPair.Value.Equals(text, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }
    }
}
