/* 2021/8/2 */
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace TerminalMonitor.Windows.Converters
{
    class ObjectNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            if (typeof(string).IsAssignableFrom(targetType))
            {
                return String.IsNullOrEmpty(value as string);
            }

            if (typeof(ICollection).IsAssignableFrom(targetType))
            {
                return (value as ICollection).Count == 0;
            }

            if (typeof(IDictionary).IsAssignableFrom(targetType))
            {
                return (value as IDictionary).Count == 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var isNull = value as bool?;
            if (isNull ?? false)
            {
                return null;
            }
            else
            {
                return new Object();
            }
        }
    }
}
