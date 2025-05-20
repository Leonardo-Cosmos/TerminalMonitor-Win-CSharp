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
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(HorizontalAlignment))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value == null || value is DBNull)
            {
                return default(HorizontalAlignment);
            }

            if (value is Int32)
            {
                return (HorizontalAlignment)value;
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                return new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value == null || value is DBNull)
            {
                return 0;
            }

            if (value is HorizontalAlignment)
            {
                return (Int32)value;
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }
    }

    class IntToVerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(VerticalAlignment))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value == null || value is DBNull)
            {
                return default(VerticalAlignment);
            }

            if (value is Int32)
            {
                return (VerticalAlignment)value;
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }

        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value == null || value is DBNull)
            {
                return 0;
            }

            if (value is VerticalAlignment)
            {
                return (Int32)value;
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }
        }
    }

    class IntToTextAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(TextAlignment))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value == null || value is DBNull)
            {
                return default(TextAlignment);
            }

            if (value is Int32)
            {
                return (TextAlignment)value;
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
            }

        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Int32))
            {
                throw new ArgumentException($"Invalid value of {nameof(targetType)}");
            }

            if (value == null || value is DBNull)
            {
                return 0;
            }

            if (value is TextAlignment)
            {
                return (Int32)value;
            }
            else
            {
                throw new ArgumentException($"Invalid type of {nameof(value)}");
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

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return "Unknown";
            }

            if (value is HorizontalAlignment horizontalAlignment)
            {
                return textDict[horizontalAlignment];
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
                return default(HorizontalAlignment);
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

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return "Unknown";
            }

            if (value is VerticalAlignment verticalAlignment)
            {
                return textDict[verticalAlignment];
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
                return default(VerticalAlignment);
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

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is DBNull)
            {
                return "Unknown";
            }

            if (value is TextAlignment textAlignment)
            {
                return textDict[textAlignment];
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
                return default(TextAlignment);
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
