/* 2021/4/22 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FilterItemView.xaml
    /// </summary>
    public partial class FilterItemView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string fieldKey;

        public string FieldKey
        {
            get { return fieldKey; }

            set
            {
                fieldKey = value;
                OnPropertyChanged();
            }
        }

        private string compareOperator;

        public string CompareOperator
        {
            get { return compareOperator; }

            set
            {
                compareOperator = value;
                OnPropertyChanged();
            }
        }

        private string comparedValue;

        public string ComparedValue
        {
            get { return comparedValue; }
            set
            {
                comparedValue = value;
                OnPropertyChanged();
            }
        }

        public FilterItemView()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
