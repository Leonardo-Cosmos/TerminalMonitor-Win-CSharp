/* 2022/4/16 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Converters
{
    class TextColorModeToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<TextColorMode, string> textDict =
            new(new Dictionary<TextColorMode, string>() {
                { TextColorMode.Static, "Static" },
                { TextColorMode.Hash, "Hash"},
                { TextColorMode.HashInverted, "Hash Inverted"},
            });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextColorMode colorMode)
            {
                return textDict[colorMode];
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
                return TextColorMode.Static;
            }
        }
    }
}
