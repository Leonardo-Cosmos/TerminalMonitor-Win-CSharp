/* 2021/7/12 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows
{
    abstract class ConditionNodeVO
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
            set
            {
                negativeMatch = value;
                OnPropertyChanged();
            }
        }

        private bool defaultMatch;

        public bool DefaultMatch
        {
            get => defaultMatch;
            set
            {
                defaultMatch = value;
                OnPropertyChanged();
            }
        }

        private bool dismissMatch;

        public bool DismissMatch
        {
            get => dismissMatch;
            set
            {
                dismissMatch = value;
                OnPropertyChanged();
            }
        }
    }
}
