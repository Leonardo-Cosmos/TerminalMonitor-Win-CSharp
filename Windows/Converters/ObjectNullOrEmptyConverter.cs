/* 2021/8/2 */
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace TerminalMonitor.Windows.Converters
{
    class ObjectNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return true;
            }

            if (value is string str)
            {
                return String.IsNullOrEmpty(str);
            }

            if (value is ICollection collection)
            {
                return collection.Count == 0;
            }

            if (value is IDictionary dictionary)
            {
                return dictionary.Count == 0;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return null;
            }

            if (value is bool isNull)
            {
                if (isNull)
                {
                    return null;
                }
                else
                {
                    return new Object();
                }
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }
    }
}
