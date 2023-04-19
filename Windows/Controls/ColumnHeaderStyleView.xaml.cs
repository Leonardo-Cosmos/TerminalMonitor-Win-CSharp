/* 2023/3/2 */
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
        public static readonly DependencyProperty ColumnHeaderStyleProperty =
            DependencyProperty.Register(nameof(ColumnHeaderStyle), typeof(ColumnHeaderStyle), typeof(ColumnHeaderStyleView),
                new PropertyMetadata(ColumnHeaderStyle.Empty, OnColumnHeaderStyleChanged));

        private ColumnHeaderStyle columnHeaderStyle;

        private readonly ColumnHeaderStyleViewDataContextVO dataContextVO = new()
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
                case nameof(ColumnHeaderStyleViewDataContextVO.ForegroundColor):
                    columnHeaderStyle.Foreground ??= (dataContextVO.ForegroundColor as SolidColorBrush).Color;
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.BackgroundColor):
                    columnHeaderStyle.Background ??= (dataContextVO.BackgroundColor as SolidColorBrush).Color;
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.CellBackgroundColor):
                    columnHeaderStyle.CellBackground ??= (dataContextVO.CellBackgroundColor as SolidColorBrush).Color;
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.HorizontalAlignment):
                    columnHeaderStyle.HorizontalAlignment = dataContextVO.HorizontalAlignment;
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.VerticalAlignment):
                    columnHeaderStyle.VerticalAlignment = dataContextVO.VerticalAlignment;
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.TextAlignment):
                    columnHeaderStyle.TextAlignment = dataContextVO.TextAlignment;
                    break;

                case nameof(ColumnHeaderStyleViewDataContextVO.EnableForeground):
                    if (!dataContextVO.EnableForeground)
                    {
                        columnHeaderStyle.Foreground = null;
                    }
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.EnableBackground):
                    if (!dataContextVO.EnableBackground)
                    {
                        columnHeaderStyle.Background = null;
                    }
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.EnableCellBackground):
                    if (!dataContextVO.EnableCellBackground)
                    {
                        columnHeaderStyle.CellBackground = null;
                    }
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.EnableHorizontalAlignment):
                    if (!dataContextVO.EnableHorizontalAlignment)
                    {
                        columnHeaderStyle.HorizontalAlignment = null;
                    }
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.EnableVerticalAlignment):
                    if (!dataContextVO.EnableVerticalAlignment)
                    {
                        columnHeaderStyle.VerticalAlignment = null;
                    }
                    break;
                case nameof(ColumnHeaderStyleViewDataContextVO.EnableTextAlignment):
                    if (!dataContextVO.EnableTextAlignment)
                    {
                        columnHeaderStyle.TextAlignment = null;
                    }
                    break;

                default:
                    break;
            }
        }

        private static void OnColumnHeaderStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var columnHeaderStyleView = dependencyObject as ColumnHeaderStyleView;
            columnHeaderStyleView.OnColumnHeaderStyleChanged(e);
        }

        private void OnColumnHeaderStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            columnHeaderStyle = e.NewValue as ColumnHeaderStyle;
            if (columnHeaderStyle != null)
            {
                dataContextVO.PropertyChanged -= OnDataContextPropertyChanged;

                dataContextVO.EnableForeground = columnHeaderStyle.Foreground != null;
                if (columnHeaderStyle.Foreground.HasValue)
                {
                    dataContextVO.ForegroundColor = new SolidColorBrush(columnHeaderStyle.Foreground.Value);
                }

                dataContextVO.EnableBackground = columnHeaderStyle.Background != null;
                if (columnHeaderStyle.Background.HasValue)
                {
                    dataContextVO.BackgroundColor = new SolidColorBrush(columnHeaderStyle.Background.Value);
                }

                dataContextVO.EnableCellBackground = columnHeaderStyle.CellBackground != null;
                if (columnHeaderStyle.CellBackground.HasValue)
                {
                    dataContextVO.CellBackgroundColor = new SolidColorBrush(columnHeaderStyle.CellBackground.Value);
                }

                dataContextVO.EnableHorizontalAlignment = columnHeaderStyle.HorizontalAlignment.HasValue;
                if (columnHeaderStyle.HorizontalAlignment.HasValue)
                {
                    dataContextVO.HorizontalAlignment = columnHeaderStyle.HorizontalAlignment.Value;
                }

                dataContextVO.EnableVerticalAlignment = columnHeaderStyle.VerticalAlignment.HasValue;
                if (columnHeaderStyle.VerticalAlignment.HasValue)
                {
                    dataContextVO.VerticalAlignment = columnHeaderStyle.VerticalAlignment.Value;
                }

                dataContextVO.EnableTextAlignment = columnHeaderStyle.TextAlignment.HasValue;
                if (columnHeaderStyle.TextAlignment.HasValue)
                {
                    dataContextVO.TextAlignment = columnHeaderStyle.TextAlignment.Value;
                }

                dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
            }
        }

        public ColumnHeaderStyle ColumnHeaderStyle
        {
            get => (ColumnHeaderStyle)GetValue(ColumnHeaderStyleProperty);
            set => SetValue(ColumnHeaderStyleProperty, value);
        }
    }
}
