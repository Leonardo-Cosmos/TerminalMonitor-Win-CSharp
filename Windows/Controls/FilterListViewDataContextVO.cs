/* 2021/7/31 */
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers.Models;

namespace TerminalMonitor.Windows.Controls
{
    class FilterListViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool negativeMatch;

        public bool NegativeMatch
        {
            get => negativeMatch;
            set { negativeMatch = value; OnPropertyChanged(); }
        }

        private bool defaultMatch;

        public bool DefaultMatch
        {
            get => defaultMatch;
            set { defaultMatch = value; OnPropertyChanged(); }
        }

        private GroupMatchMode matchMode;

        public GroupMatchMode MatchMode
        {
            get => matchMode;
            set { matchMode = value; OnPropertyChanged(); }
        }
    }
}
