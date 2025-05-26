/* 2021/5/30 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Windows
{
    class CommandDetailWindowDataContextVO : INotifyPropertyChanged
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

        private string? startFile;

        public string? StartFile
        {
            get => startFile;
            set { startFile = value; OnPropertyChanged(); }
        }

        private string? arguments;

        public string? Arguments
        {
            get => arguments;
            set { arguments = value; OnPropertyChanged(); }
        }

        private string? workDirectory;

        public string? WorkDirectory
        {
            get => workDirectory;
            set { workDirectory = value; OnPropertyChanged(); }
        }
    }
}
