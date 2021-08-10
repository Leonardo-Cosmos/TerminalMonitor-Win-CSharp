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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GroupMatchMode matchMode)
            {
                return textDict[matchMode];
            }
            else
            {
                return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return textDict
                    .FirstOrDefault(kvPair => kvPair.Value.Equals(text, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return GroupMatchMode.All;
            }
        }
    }
}
