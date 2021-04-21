/* 2021/4/21 */
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

namespace TerminalMonitor.Controls
{
    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string field;

        public string Field
        {
            get
            {
                return field;
            }

            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        private string compareOperator;

        public string Operator
        {
            get
            {
                return compareOperator;
            }

            set
            {
                compareOperator = value;
                OnPropertyChanged();
            }
        }

        public FilterView()
        {
            InitializeComponent();
        }
    }
}
