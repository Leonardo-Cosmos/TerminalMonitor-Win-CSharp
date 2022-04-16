/* 2021/7/12 */
using System.Collections.ObjectModel;
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

        public ObservableCollection<ConditionNodeVO> Siblings { get; set; }

        private bool isInverted;

        public bool IsInverted
        {
            get => isInverted;
            set
            {
                isInverted = value;
                OnPropertyChanged();
            }
        }

        private bool defaultResult;

        public bool DefaultResult
        {
            get => defaultResult;
            set
            {
                defaultResult = value;
                OnPropertyChanged();
            }
        }

        private bool isDisabled;

        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;
                OnPropertyChanged();
            }
        }
    }
}
