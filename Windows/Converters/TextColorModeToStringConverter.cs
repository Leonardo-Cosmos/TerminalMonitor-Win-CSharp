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
                { TextColorMode.Hash, "Hash" },
                { TextColorMode.HashInverted, "Hash Inverted" },
                { TextColorMode.HashSymmetric, "Hash Symmetric" },
            });

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return "Unknown";
            }

            if (value is TextColorMode colorMode)
            {
                return textDict[colorMode];
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
                return default(TextColorMode);
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
