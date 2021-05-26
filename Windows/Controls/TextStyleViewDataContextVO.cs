/* 2021/5/23 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace TerminalMonitor.Windows.Controls
{
    class TextStyleViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Brush foreground;

        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                foreground = value;
                OnPropertyChanged();
            }
        }

        private Brush background;

        public Brush Background
        {
            get { return background; }
            set
            {
                background = value;
                OnPropertyChanged();
            }
        }
    }
}
