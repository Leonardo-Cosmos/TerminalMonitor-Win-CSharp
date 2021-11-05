/* 2021/5/24 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows
{
    class FieldDisplayDetailWindowDataContextVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string fieldKey;

        public string FieldKey
        {
            get => fieldKey;
            set { fieldKey = value; OnPropertyChanged(); }
        }

        private bool customizeStyle;

        public bool CustomizeStyle
        {
            get => customizeStyle;
            set { customizeStyle = value; OnPropertyChanged(); }
        }

        private TextStyle style;

        public TextStyle Style
        {
            get => style;
            set { style = value; OnPropertyChanged(); }
        }

        private bool isAnyConditionSelected;

        public bool IsAnyConditionSelected
        {
            get => isAnyConditionSelected;
            set
            {
                isAnyConditionSelected = value;
                OnPropertyChanged();
            }
        }

        private readonly ObservableCollection<TextStyleCondition> conditions = new();

        public ObservableCollection<TextStyleCondition> Conditions
        {
            get => conditions;
        }

    }
}
