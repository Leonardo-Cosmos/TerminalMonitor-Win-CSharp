/* 2021/5/23 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TextStyleView.xaml
    /// </summary>
    public partial class TextStyleView : UserControl
    {
        public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register(nameof(TextStyle), typeof(TextStyle), typeof(TextStyleView),
                new PropertyMetadata(TextStyle.Empty, OnTextStyleChanged));

        private TextStyle textStyle;

        private readonly TextStyleViewDataContextVO dataContextVO = new()
        {
            ForegroundColor = Brushes.Black,
            BackgroundColor = Brushes.White,
            CellBackgroundColor = Brushes.White,
            MaxWidth = 100,
            MaxHeight = 50,
        };

        public TextStyleView()
        {
            InitializeComponent();

            pnl.DataContext = dataContextVO;
            dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
        }

        private void RctForegroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ColorDialogHelper.ShowColorDialog(dataContextVO.ForegroundColor as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.ForegroundColor = brush;
            }
        }

        private void RctBackgroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ColorDialogHelper.ShowColorDialog(dataContextVO.BackgroundColor as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.BackgroundColor = brush;
            }
        }

        private void RctCellBackgroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ColorDialogHelper.ShowColorDialog(dataContextVO.CellBackgroundColor as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.CellBackgroundColor = brush;
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TextStyleViewDataContextVO.ForegroundColor):
                    textStyle.Foreground ??= new TextColorConfig();
                    textStyle.Foreground.Color = (dataContextVO.ForegroundColor as SolidColorBrush).Color;
                    break;
                case nameof(TextStyleViewDataContextVO.ForegroundColorMode):
                    textStyle.Foreground ??= new TextColorConfig();
                    textStyle.Foreground.Mode = dataContextVO.ForegroundColorMode;
                    textStyle.Foreground.Color = dataContextVO.IsForegroundColorStatic ?
                        (dataContextVO.ForegroundColor as SolidColorBrush).Color : null;
                    break;
                case nameof(TextStyleViewDataContextVO.BackgroundColor):
                    textStyle.Background ??= new TextColorConfig();
                    textStyle.Background.Color = (dataContextVO.BackgroundColor as SolidColorBrush).Color;
                    break;
                case nameof(TextStyleViewDataContextVO.BackgroundColorMode):
                    textStyle.Background ??= new TextColorConfig();
                    textStyle.Background.Mode = dataContextVO.BackgroundColorMode;
                    textStyle.Background.Color = dataContextVO.IsBackgroundColorStatic ?
                        (dataContextVO.BackgroundColor as SolidColorBrush).Color : null;
                    break;
                case nameof(TextStyleViewDataContextVO.CellBackgroundColor):
                    textStyle.CellBackground ??= new TextColorConfig();
                    textStyle.CellBackground.Color = (dataContextVO.CellBackgroundColor as SolidColorBrush).Color;
                    break;
                case nameof(TextStyleViewDataContextVO.CellBackgroundColorMode):
                    textStyle.CellBackground ??= new TextColorConfig();
                    textStyle.CellBackground.Mode = dataContextVO.CellBackgroundColorMode;
                    textStyle.CellBackground.Color = dataContextVO.IsCellBackgroundColorStatic ?
                        (dataContextVO.CellBackgroundColor as SolidColorBrush).Color : null;
                    break;
                case nameof(TextStyleViewDataContextVO.HorizontalAlignment):
                    textStyle.HorizontalAlignment = dataContextVO.HorizontalAlignment;
                    break;
                case nameof(TextStyleViewDataContextVO.VerticalAlignment):
                    textStyle.VerticalAlignment = dataContextVO.VerticalAlignment;
                    break;
                case nameof(TextStyleViewDataContextVO.TextAlignment):
                    textStyle.TextAlignment = dataContextVO.TextAlignment;
                    break;
                case nameof(TextStyleViewDataContextVO.MaxWidth):
                    textStyle.MaxWidth = dataContextVO.MaxWidth;
                    break;
                case nameof(TextStyleViewDataContextVO.MaxHeight):
                    textStyle.MaxHeight = dataContextVO.MaxHeight;
                    break;
                case nameof(TextStyleViewDataContextVO.TextWrapping):
                    textStyle.TextWrapping = dataContextVO.TextWrapping;
                    break;

                case nameof(TextStyleViewDataContextVO.EnableForeground):
                    if (dataContextVO.EnableForeground)
                    {
                        textStyle.Foreground ??= new TextColorConfig();
                        textStyle.Foreground.Mode = dataContextVO.ForegroundColorMode;
                        textStyle.Foreground.Color = dataContextVO.IsForegroundColorStatic ?
                            (dataContextVO.ForegroundColor as SolidColorBrush).Color : null;
                    }
                    else
                    {
                        textStyle.Foreground = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableBackground):
                    if (dataContextVO.EnableBackground)
                    {
                        textStyle.Background ??= new TextColorConfig();
                        textStyle.Background.Mode = dataContextVO.BackgroundColorMode;
                        textStyle.Background.Color = dataContextVO.IsBackgroundColorStatic ?
                            (dataContextVO.BackgroundColor as SolidColorBrush).Color : null;
                    }
                    else
                    {
                        textStyle.Background = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableCellBackground):
                    if (dataContextVO.EnableCellBackground)
                    {
                        textStyle.CellBackground ??= new TextColorConfig();
                        textStyle.CellBackground.Mode = dataContextVO.CellBackgroundColorMode;
                        textStyle.CellBackground.Color = dataContextVO.IsCellBackgroundColorStatic ?
                            (dataContextVO.CellBackgroundColor as SolidColorBrush).Color : null;
                    }
                    else
                    {
                        textStyle.CellBackground = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableHorizontalAlignment):
                    if (dataContextVO.EnableHorizontalAlignment)
                    {
                        textStyle.HorizontalAlignment = dataContextVO.HorizontalAlignment;
                    }
                    else
                    {
                        textStyle.HorizontalAlignment = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableVerticalAlignment):
                    if (dataContextVO.EnableVerticalAlignment)
                    {
                        textStyle.VerticalAlignment = dataContextVO.VerticalAlignment;
                    }
                    else
                    {
                        textStyle.VerticalAlignment = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableTextAlignment):
                    if (dataContextVO.EnableTextAlignment)
                    {
                        textStyle.TextAlignment = dataContextVO.TextAlignment;
                    }
                    else
                    {
                        textStyle.TextAlignment = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableMaxWidth):
                    if (dataContextVO.EnableMaxWidth)
                    {
                        textStyle.MaxWidth = dataContextVO.MaxWidth;
                    }
                    else
                    {
                        textStyle.MaxWidth = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableMaxHeight):
                    if (dataContextVO.EnableMaxHeight)
                    {
                        textStyle.MaxHeight = dataContextVO.MaxHeight;
                    }
                    else
                    {
                        textStyle.MaxHeight = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableTextWrapping):
                    if (dataContextVO.EnableTextWrapping)
                    {
                        textStyle.TextWrapping = dataContextVO.TextWrapping;
                    }
                    else
                    {
                        textStyle.TextWrapping = null;
                    }
                    break;

                default:
                    break;
            }
        }

        private static void OnTextStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var textStyleView = dependencyObject as TextStyleView;
            textStyleView.OnTextStyleChanged(e);
        }

        private void OnTextStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            textStyle = e.NewValue as TextStyle;
            if (textStyle != null)
            {
                dataContextVO.PropertyChanged -= OnDataContextPropertyChanged;

                dataContextVO.EnableForeground = textStyle.Foreground != null;
                if (textStyle.Foreground != null)
                {
                    dataContextVO.ForegroundColorMode = textStyle.Foreground.Mode;

                    if (textStyle.Foreground.Color.HasValue)
                    {
                        dataContextVO.ForegroundColor = new SolidColorBrush(textStyle.Foreground.Color.Value);
                    }
                }

                dataContextVO.EnableBackground = textStyle.Background != null;
                if (textStyle.Background != null)
                {
                    dataContextVO.BackgroundColorMode = textStyle.Background.Mode;

                    if (textStyle.Background.Color.HasValue)
                    {
                        dataContextVO.BackgroundColor = new SolidColorBrush(textStyle.Background.Color.Value);
                    }
                }

                dataContextVO.EnableCellBackground = textStyle.CellBackground != null;
                if (textStyle.CellBackground != null)
                {
                    dataContextVO.CellBackgroundColorMode = textStyle.CellBackground.Mode;

                    if (textStyle.CellBackground.Color.HasValue)
                    {
                        dataContextVO.CellBackgroundColor = new SolidColorBrush(textStyle.CellBackground.Color.Value);
                    }
                }

                dataContextVO.EnableHorizontalAlignment = textStyle.HorizontalAlignment.HasValue;
                if (textStyle.HorizontalAlignment.HasValue)
                {
                    dataContextVO.HorizontalAlignment = textStyle.HorizontalAlignment.Value;
                }

                dataContextVO.EnableVerticalAlignment = textStyle.VerticalAlignment.HasValue;
                if (textStyle.VerticalAlignment.HasValue)
                {
                    dataContextVO.VerticalAlignment = textStyle.VerticalAlignment.Value;
                }

                dataContextVO.EnableTextAlignment = textStyle.TextAlignment.HasValue;
                if (textStyle.TextAlignment.HasValue)
                {
                    dataContextVO.TextAlignment = textStyle.TextAlignment.Value;
                }

                dataContextVO.EnableMaxWidth = textStyle.MaxWidth.HasValue;
                if (textStyle.MaxWidth.HasValue)
                {
                    dataContextVO.MaxWidth = (int)textStyle.MaxWidth.Value;
                }

                dataContextVO.EnableMaxHeight = textStyle.MaxHeight.HasValue;
                if (textStyle.MaxHeight.HasValue)
                {
                    dataContextVO.MaxHeight = (int)textStyle.MaxHeight.Value;
                }

                dataContextVO.EnableTextWrapping = textStyle.TextWrapping.HasValue;
                if (textStyle.TextWrapping.HasValue)
                {
                    dataContextVO.TextWrapping = textStyle.TextWrapping.Value;
                }

                dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
            }
        }

        public TextStyle TextStyle
        {
            get => (TextStyle)GetValue(TextStyleProperty);
            set => SetValue(TextStyleProperty, value);
        }
    }
}
