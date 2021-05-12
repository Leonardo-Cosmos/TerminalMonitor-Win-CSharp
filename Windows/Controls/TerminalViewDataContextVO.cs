/* 2021/5/12 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TerminalMonitor.Windows.Controls
{
    class TerminalViewDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool autoScroll;

        public bool AutoScroll
        {
            get { return autoScroll; }

            set
            {
                autoScroll = value;
                OnPropertyChanged();
            }
        }
    }
}
