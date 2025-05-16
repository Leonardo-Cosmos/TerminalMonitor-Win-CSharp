/* 2024/3/25 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TerminalMonitor.Windows.Converters
{
    class IntToTextWrappingConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(TextWrapping))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value is Int32)
            {
                return (TextWrapping)value;
            }
            else
            {
                return TextWrapping.NoWrap;
            }

        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value is TextWrapping)
            {
                return (Int32)value;
            }
            else
            {
                return (Int32)TextWrapping.NoWrap;
            }
        }
    }

    class TextWrappingToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<TextWrapping, string> textDict =
            new(new Dictionary<TextWrapping, string>()
            {
                { TextWrapping.NoWrap, "No Wrap" },
                { TextWrapping.Wrap, "Wrap" },
            });

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "Unknown";
            }

            if (value is TextWrapping textWrapping)
            {
                return textDict[textWrapping];
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return TextWrapping.NoWrap;
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
