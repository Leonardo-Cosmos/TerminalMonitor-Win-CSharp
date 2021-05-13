/* 2021/4/20 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalMonitor.Matchers;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    class TerminalLineVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string PlainText { get; set; }

        public Dictionary<string, object> ParsedFieldDict { get; set; }

        public List<TerminalLineFieldVO> ParsedFields { get; set; }

        public bool Matched
        {
            get;
            set;
        }

    }
}
