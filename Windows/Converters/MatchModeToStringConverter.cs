/* 2021/7/13 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Windows.Converters
{
    class MatchModeToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<GroupMatchMode, string> textDict =
            new(new Dictionary<GroupMatchMode, string>() {
                { GroupMatchMode.All, "All" },
                { GroupMatchMode.Any, "Any"},
            });

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return "Unknown";
            }

            if (value is GroupMatchMode matchMode)
            {
                return textDict[matchMode];
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return default(GroupMatchMode);
            }

            if (value is string text)
            {
                return textDict
                    .FirstOrDefault(kvPair => kvPair.Value.Equals(text, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }
    }
}
