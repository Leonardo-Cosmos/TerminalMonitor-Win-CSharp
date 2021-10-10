/* 2021/5/23 */
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
            Foreground = Brushes.Black,
            Background = Brushes.White,
            CellBackground = Brushes.White,
        };

        public TextStyleView()
        {
            InitializeComponent();

            pnl.DataContext = dataContextVO;
            dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
        }

        private static SolidColorBrush ShowColorDialog(SolidColorBrush brush)
        {
            var color = brush.Color;
            System.Windows.Forms.ColorDialog colorDialog = new();

            colorDialog.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedColor = colorDialog.Color;
                color = Color.FromArgb(selectedColor.A,
                    selectedColor.R, selectedColor.G, selectedColor.B);
                return new SolidColorBrush(color);
            }
            else
            {
                return null;
            }
        }

        private void RctForeground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ShowColorDialog(dataContextVO.Foreground as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.Foreground = brush;
            }
        }

        private void RctBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ShowColorDialog(dataContextVO.Background as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.Background = brush;
            }
        }

        private void RctCellBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brush = ShowColorDialog(dataContextVO.CellBackground as SolidColorBrush);
            if (brush != null)
            {
                dataContextVO.CellBackground = brush;
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TextStyleViewDataContextVO.Foreground):
                    textStyle.Foreground = (dataContextVO.Foreground as SolidColorBrush).Color;
                    break;
                case nameof(TextStyleViewDataContextVO.Background):
                    textStyle.Background = (dataContextVO.Background as SolidColorBrush).Color;
                    break;
                case nameof(TextStyleViewDataContextVO.CellBackground):
                    textStyle.CellBackground = (dataContextVO.CellBackground as SolidColorBrush).Color;
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
                dataContextVO.EnableForeground = textStyle.Foreground.HasValue;
                if (textStyle.Foreground.HasValue)
                {
                    dataContextVO.Foreground = new SolidColorBrush(textStyle.Foreground.Value);
                }

                dataContextVO.EnableBackground = textStyle.Background.HasValue;
                if (textStyle.Background.HasValue)
                {
                    dataContextVO.Background = new SolidColorBrush(textStyle.Background.Value);
                }

                dataContextVO.EnableCellBackground = textStyle.CellBackground.HasValue;
                if (textStyle.CellBackground.HasValue)
                {
                    dataContextVO.CellBackground = new SolidColorBrush(textStyle.CellBackground.Value);
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
            }
        }

        public TextStyle TextStyle
        {
            get => (TextStyle)GetValue(TextStyleProperty);
            set => SetValue(TextStyleProperty, value);
        }
    }
}
