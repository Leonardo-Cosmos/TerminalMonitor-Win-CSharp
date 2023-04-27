/* 2021/5/12 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows.Controls
{
    class FieldListItemVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Id { get; init; }

        private string fieldKey;

        public string FieldKey
        {
            get { return fieldKey; }

            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private bool hidden;

        public bool Hidden
        {
            get { return hidden; }

            set
            {
                hidden = value;
                OnPropertyChanged();
            }
        }
    }
}
