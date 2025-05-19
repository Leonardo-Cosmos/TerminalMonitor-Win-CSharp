/* 2021/6/25 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Windows
{
    class InputWindowDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? message;

        public required string Message
        {
            get => message!;
            set { message = value; OnPropertyChanged(); }
        }

        private string? text;

        public required string Text
        {
            get => text!;
            set { text = value; OnPropertyChanged(); }
        }
    }
}
