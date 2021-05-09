/* 2021/4/24 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static TerminalMonitor.Checkers.TextChecker;

namespace TerminalMonitor.Windows.Converters
{
    class CheckOperatorToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<CheckOperator, string> opTextDict = 
            new(new Dictionary<CheckOperator, string>() {
                { CheckOperator.None, "--" },
                { CheckOperator.Equals, "equals"},
                { CheckOperator.Contains, "contains" },
                { CheckOperator.StartsWith, "starts with" },
                { CheckOperator.EndsWith, "ends with" },
                { CheckOperator.Matches, "matches" },
            });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CheckOperator checkOperator)
            {
                return opTextDict[checkOperator];
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
                return CheckOperator.None;
            }
        }
    }
}
