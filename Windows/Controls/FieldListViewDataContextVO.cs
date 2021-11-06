/* 2021/11/6 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows.Controls
{
    class FieldListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isAnySelected;

        public bool IsAnySelected
        {
            get => isAnySelected;
            set
            {
                isAnySelected = value;
                OnPropertyChanged();
            }
        }

        private bool isAnyFieldInClipboard;

        public bool IsAnyFieldInClipboard
        {
            get => isAnyFieldInClipboard;
            set
            {
                isAnyFieldInClipboard = value;
                OnPropertyChanged();
            }
        }
    }
}
