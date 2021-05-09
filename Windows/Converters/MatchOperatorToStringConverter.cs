/* 2021/4/24 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static TerminalMonitor.Matchers.TextMatcher;

namespace TerminalMonitor.Windows.Converters
{
    class MatchOperatorToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<MatchOperator, string> opTextDict = 
            new(new Dictionary<MatchOperator, string>() {
                { MatchOperator.None, "--" },
                { MatchOperator.Equals, "equals"},
                { MatchOperator.Contains, "contains" },
                { MatchOperator.StartsWith, "starts with" },
                { MatchOperator.EndsWith, "ends with" },
                { MatchOperator.Matches, "matches" },
            });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MatchOperator matchOperator)
            {
                return opTextDict[matchOperator];
            }
            else
            {
                return "?";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return opTextDict
                    .FirstOrDefault(kvPair => kvPair.Value.Equals(text, StringComparison.OrdinalIgnoreCase));
            } else
            {
                return MatchOperator.None;
            }
        }
    }
}
