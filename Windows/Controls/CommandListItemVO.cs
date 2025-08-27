/* 2021/5/30 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TerminalMonitor.Windows.Controls
{
    class CommandListItemVO : INotifyPropertyChanged
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

        private Visibility stopButtonVisibility = Visibility.Hidden;

        public Visibility StopButtonVisibility
        {
            get => stopButtonVisibility;
            set { stopButtonVisibility = value; OnPropertyChanged(); }
        }
    }
}
