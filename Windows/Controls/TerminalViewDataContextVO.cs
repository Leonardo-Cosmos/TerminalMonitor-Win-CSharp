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
            get => autoScroll;

            set
            {
                autoScroll = value;
                OnPropertyChanged();
            }
        }

        private int foundCount;

        public int FoundCount
        {
            get => foundCount;

            set
            {
                foundCount = value;
                OnPropertyChanged();
            }
        }

        private string foundSelectedNumber;

        public string FoundSelectedNumber
        {
            get => foundSelectedNumber;

            set
            {
                foundSelectedNumber = value;
                OnPropertyChanged();
            }
        }
    }
}
