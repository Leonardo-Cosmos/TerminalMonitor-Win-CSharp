/* 2021/5/23 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private TextStyle style;

        public TextStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TextStyleCondition> conditions; 

        public ObservableCollection<TextStyleCondition> Conditions
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
