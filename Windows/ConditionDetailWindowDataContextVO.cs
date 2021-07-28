/* 2021/7/28 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows
{
    class ConditionDetailWindowDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string conditionName;

        public string ConditionName
        {
            get => conditionName;
            set { conditionName = value; OnPropertyChanged(); }
        }
    }
}
