/* 2021/10/8 */
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TerminalMonitor.Windows.Converters
{
    public class IntToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(HorizontalAlignment))
            {
                return null;
            }

            if (value is Int32)
            {
                return (HorizontalAlignment)value;
            }
            else
            {
                return HorizontalAlignment.Stretch;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                return null;
            }

            if (value is HorizontalAlignment)
            {
                return (Int32)value;
            }
            else
            {
                return 0;
            }
        }
    }

    public class IntToVerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(VerticalAlignment))
            {
                return null;
            }

            if (value is Int32)
            {
                return (VerticalAlignment)value;
            }
            else
            {
                return VerticalAlignment.Stretch;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                return null;
            }

            if (value is VerticalAlignment)
            {
                return (Int32)value;
            }
            else
            {
                return 0;
            }
        }
    }
}
