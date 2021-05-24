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
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FieldListView.xaml
    /// </summary>
    public partial class FieldListView : UserControl
    {
        private readonly FieldItemVO currentField = new();

        private readonly ObservableCollection<FieldItemVO> fieldVOs = new();

        private readonly List<FieldDisplayDetail> fields = new();

        public FieldListView()
        {
            InitializeComponent();

            DataContext = currentField;
            lstFields.ItemsSource = fieldVOs;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            FieldDisplayDetailWindow window = new();
            if (window.ShowDialog() ?? false)
            {
                var field = window.Field;

                FieldItemVO item = new()
                {
                    FieldKey = field.FieldKey
                };
                fieldVOs.Add(item);
                lstFields.SelectedItem = item;

                fields.Add(field);
            }
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);

                var field = fields[index];
                FieldDisplayDetailWindow window = new()
                {
                    Field = field,
                };
                if (window.ShowDialog() ?? false)
                {
                    field = window.Field;
                    fields[index] = field;

                    fieldVOs[index].FieldKey = field.FieldKey;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);
                fieldVOs.RemoveAt(index);

                fields.RemoveAt(index);
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

                    var field = fields[index];
                    fields.RemoveAt(index);
                    fields.Insert(index - 1, field);
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

                    var field = fields[index];
                    fields.RemoveAt(index);
                    fields.Insert(index + 1, field);
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
