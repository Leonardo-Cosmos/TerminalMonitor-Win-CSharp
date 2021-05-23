/* 2021/5/23 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    class TextStyleConditionViewDataContextVO
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private TextStyle defaultStyle;

        public TextStyle DefaultStyle
        {
            get { return defaultStyle; }
            set
            {
                defaultStyle = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<TextStyleCondition> conditions; 

        public IEnumerable<TextStyleCondition> Conditions
        {
            get { return conditions; }
            set
            {
                conditions = value;
                OnPropertyChanged();
            }
        }

    }
}
