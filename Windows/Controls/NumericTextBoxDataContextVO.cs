/* 2024/3/19 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows.Controls
{
    class NumericTextBoxDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string numberText;

        public string NumberText
        {
            get => numberText;
            set
            {
                numberText = value;
                OnPropertyChanged();
            }
        }
    }
}
