/* 2021/5/23 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    class TextStyleViewDataContextVO : INotifyPropertyChanged
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

        public Brush Foreground
        {
            get => foreground;
            set
            {
                foreground = value;
                OnPropertyChanged();
            }
        }

        private TextColorMode foregroundColorMode;

        public TextColorMode ForegroundColorMode
        {
            get => foregroundColorMode;
            set
            {
                foregroundColorMode = value;
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

        public Brush Background
        {
            get => background;
            set
            {
                background = value;
                OnPropertyChanged();
            }
        }

        private TextColorMode backgroundColorMode;

        public TextColorMode BackgroundColorMode
        {
            get => backgroundColorMode;
            set
            {
                backgroundColorMode = value;
                OnPropertyChanged();
            }
        }

        private bool enableCellbackground;

        public bool EnableCellBackground
        {
            get => enableCellbackground;
            set
            {
                enableCellbackground = value;
                OnPropertyChanged();
            }
        }

        private Brush cellBackground;

        public Brush CellBackground
        {
            get => cellBackground;
            set
            {
                cellBackground = value;
                OnPropertyChanged();
            }
        }

        private TextColorMode cellBackgroundColorMode;

        public TextColorMode CellBackgroundColorMode
        {
            get => cellBackgroundColorMode;
            set
            {
                cellBackgroundColorMode = value;
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
