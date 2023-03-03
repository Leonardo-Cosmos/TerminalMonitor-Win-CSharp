﻿/* 2023/3/2 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ColumnHeaderStyleView.xaml
    /// </summary>
    public partial class ColumnHeaderStyleView : UserControl
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
        };

        public ColumnHeaderStyleView()
        {
            InitializeComponent();

            pnl.DataContext = dataContextVO;
            dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
        }

        private void RctForegroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ShowColorDialog(dataContextVO.ForegroundColor as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.ForegroundColor = brush;
            }
        }

        private void RctBackgroundColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ShowColorDialog(dataContextVO.BackgroundColor as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.BackgroundColor = brush;
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

                case nameof(TextStyleViewDataContextVO.EnableForeground):
                    if (!dataContextVO.EnableForeground)
                    {
                        textStyle.Foreground = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableBackground):
                    if (!dataContextVO.EnableBackground)
                    {
                        textStyle.Background = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableCellBackground):
                    if (!dataContextVO.EnableCellBackground)
                    {
                        textStyle.CellBackground = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableHorizontalAlignment):
                    if (!dataContextVO.EnableHorizontalAlignment)
                    {
                        textStyle.HorizontalAlignment = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableVerticalAlignment):
                    if (!dataContextVO.EnableVerticalAlignment)
                    {
                        textStyle.VerticalAlignment = null;
                    }
                    break;
                case nameof(TextStyleViewDataContextVO.EnableTextAlignment):
                    if (!dataContextVO.EnableTextAlignment)
                    {
                        textStyle.TextAlignment = null;
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
