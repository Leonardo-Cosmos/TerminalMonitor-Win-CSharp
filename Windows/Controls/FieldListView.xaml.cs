/* 2021/5/12 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for FieldListView.xaml
    /// </summary>
    public partial class FieldListView : UserControl
    {
        private readonly FieldItemVO currentField = new();

        private readonly ObservableCollection<FieldItemVO> fieldVOs = new();

        public FieldListView()
        {
            InitializeComponent();

            DataContext = currentField;
            lstFields.ItemsSource = fieldVOs;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            FieldItemVO item = new()
            {
                FieldKey = currentField.FieldKey
            };
            fieldVOs.Add(item);
            lstFields.SelectedItem = item;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldItemVO selectedItem)
            {
                fieldVOs.Remove(selectedItem);
            }
        }

        private void BtnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);
                if (index > 0)
                {
                    fieldVOs.Remove(selectedItem);
                    fieldVOs.Insert(index - 1, selectedItem);

                    lstFields.SelectedItem = selectedItem;
                }
            }
        }

        private void BtnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);
                if (index < fieldVOs.Count - 1)
                {
                    fieldVOs.Remove(selectedItem);
                    fieldVOs.Insert(index + 1, selectedItem);

                    lstFields.SelectedItem = selectedItem;
                }
            }
        }

        private void LstFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldItemVO selectedItem)
            {
                currentField.FieldKey = selectedItem.FieldKey;
            }
            else
            {
                currentField.FieldKey = String.Empty;
            }
        }

        internal IEnumerable<string> FieldKeys
        {
            get
            {
                return fieldVOs.Select(fieldVO => fieldVO.FieldKey);
            }

            set
            {
                fieldVOs.Clear();
                if (value == null)
                {
                    return;
                }
                value.Select(fieldKey => new FieldItemVO()
                {
                    FieldKey = fieldKey,
                }).ToList().ForEach(fieldVO => fieldVOs.Add(fieldVO));
            }
        }
    }
}
