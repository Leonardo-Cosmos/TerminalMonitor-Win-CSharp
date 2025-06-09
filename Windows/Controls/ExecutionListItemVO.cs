/* 2021/6/10 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TerminalMonitor.Windows.Controls
{
    class ExecutionListItemVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? name;

        public required string Name
        {
            get => name!;
            set { name = value; OnPropertyChanged(); }
        }

        private Guid id;

        public required Guid Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }
    }
}
