/* 2021/11/2 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows.Controls
{
    class CommandListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isSingleSelected;

        public bool IsSingleSelected
        {
            get => isSingleSelected;
            set
            {
                isSingleSelected = value;
                OnPropertyChanged();
            }
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
    }
}
