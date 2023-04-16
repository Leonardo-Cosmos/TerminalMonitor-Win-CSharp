/* 2023/3/1 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace TerminalMonitor.Windows.Controls
{
    class ColumnHeaderStyleViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
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

        private Brush foreground;

        public Brush ForegroundColor
        {
            get => foreground;
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

        private Brush background;

        public Brush BackgroundColor
        {
            get => background;
            set
            {
                background = value;
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
