/* 2021/10/8 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TerminalMonitor.Windows.Converters
{
    class IntToHorizontalAlignmentConverter : IValueConverter
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
                return HorizontalAlignment.Left;
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

    class IntToVerticalAlignmentConverter : IValueConverter
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
                return VerticalAlignment.Top;
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

    class IntToTextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(TextAlignment))
            {
                return null;
            }

            if (value is Int32)
            {
                return (TextAlignment)value;
            }
            else
            {
                return TextAlignment.Left;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                return null;
            }

            if (value is TextAlignment)
            {
                return (Int32)value;
            }
            else
            {
                return 0;
            }
        }
    }

    class HorizontalAlignmentToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<HorizontalAlignment, string> textDict =
            new(new Dictionary<HorizontalAlignment, string>()
            {
                { HorizontalAlignment.Left, "Left" },
                { HorizontalAlignment.Center, "Center" },
                { HorizontalAlignment.Right, "Right" },
                { HorizontalAlignment.Stretch, "Stretch" },
            });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignment horizontalAlignment)
            {
                return textDict[horizontalAlignment];
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
                return HorizontalAlignment.Left;
            }
        }
    }

    class VerticalAlignmentToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<VerticalAlignment, string> textDict =
            new(new Dictionary<VerticalAlignment, string>()
            {
                { VerticalAlignment.Top, "Top" },
                { VerticalAlignment.Center, "Center" },
                { VerticalAlignment.Bottom, "Bottom" },
                { VerticalAlignment.Stretch, "Stretch" },
            });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VerticalAlignment verticalAlignment)
            {
                return textDict[verticalAlignment];
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
                return VerticalAlignment.Top;
            }
        }
    }

    class TextAlignmentToStringConverter : IValueConverter
    {
        private static readonly ReadOnlyDictionary<TextAlignment, string> textDict =
            new(new Dictionary<TextAlignment, string>()
            {
                { TextAlignment.Left, "Left" },
                { TextAlignment.Right, "Right" },
                { TextAlignment.Center, "Center" },
                { TextAlignment.Justify, "Justify" },
            });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextAlignment textAlignment)
            {
                return textDict[textAlignment];
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
                return TextAlignment.Left;
            }
        }
    }
}
