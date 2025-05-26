/* 2023/3/1 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace TerminalMonitor.Windows.Controls
{
    class ColumnHeaderStyleViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool enableForeground;

        public bool EnableForeground
        {
            get => enableForeground;
            set
            {
                enableForeground = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush? foreground;

        public required SolidColorBrush ForegroundColor
        {
            get => foreground!;
            set
            {
                foreground = value;
                OnPropertyChanged();
            }
        }

        private bool enableBackground;

        public bool EnableBackground
        {
            get => enableBackground;
            set
            {
                enableBackground = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush? background;

        public required SolidColorBrush BackgroundColor
        {
            get => background!;
            set
            {
                background = value;
                OnPropertyChanged();
            }
        }

        private bool enableCellBackground;

        public bool EnableCellBackground
        {
            get => enableCellBackground;
            set
            {
                enableCellBackground = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush? cellBackground;

        public required SolidColorBrush CellBackgroundColor
        {
            get => cellBackground!;
            set
            {
                cellBackground = value;
                OnPropertyChanged();
            }
        }

        private bool enableHorizontalAlignment;

        public bool EnableHorizontalAlignment
        {
            get => enableHorizontalAlignment;
            set
            {
                enableHorizontalAlignment = value;
                OnPropertyChanged();
            }
        }

        private HorizontalAlignment horizontalAlignment;

        public HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                horizontalAlignment = value;
                OnPropertyChanged();
            }
        }

        private bool enableVerticalAlignment;

        public bool EnableVerticalAlignment
        {
            get => enableVerticalAlignment;
            set
            {
                enableVerticalAlignment = value;
                OnPropertyChanged();
            }
        }

        private VerticalAlignment verticalAlignment;

        public VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                verticalAlignment = value;
                OnPropertyChanged();
            }
        }

        private bool enableTextAlignment;

        public bool EnableTextAlignment
        {
            get => enableTextAlignment;
            set
            {
                enableTextAlignment = value;
                OnPropertyChanged();
            }
        }

        private TextAlignment textAlignment;

        public TextAlignment TextAlignment
        {
            get => textAlignment;
            set
            {
                textAlignment = value;
                OnPropertyChanged();
            }
        }
    }
}
