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
using TerminalMonitor.Clipboard;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FieldListView.xaml
    /// </summary>
    public partial class FieldListView : UserControl
    {
        private readonly ObservableCollection<FieldListItemVO> fieldVOs = new();

        private readonly List<FieldDisplayDetail> fields = new();

        public FieldListView()
        {
            InitializeComponent();

            lstFields.ItemsSource = fieldVOs;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var existingFieldKeys = fields
                    .Select(field => field.FieldKey);
            FieldDisplayDetailWindow window = new()
            {
                ExistingFieldKeys = existingFieldKeys,
            };
            if (window.ShowDialog() ?? false)
            {
                var field = window.FieldDetail;

                FieldListItemVO item = new()
                {
                    Id = field.Id,
                    FieldKey = field.FieldKey
                };
                fieldVOs.Add(item);
                lstFields.SelectedItem = item;

                fields.Add(field);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldListItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);
                fieldVOs.RemoveAt(index);

                fields.RemoveAt(index);
            }
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldListItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);

                var field = fields[index];
                var existingFieldKeys = fields
                    .Select(field => field.FieldKey)
                    .Where(fieldKey => fieldKey != field.FieldKey);
                FieldDisplayDetailWindow window = new()
                {
                    FieldDetail = field,
                    ExistingFieldKeys = existingFieldKeys,
                };
                if (window.ShowDialog() ?? false)
                {
                    field = window.FieldDetail;
                    fields[index] = field;

                    fieldVOs[index].FieldKey = field.FieldKey;
                }
            }
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldListItemVO selectedItem)
            {
                var index = fieldVOs.IndexOf(selectedItem);

                var field = fields[index];
                FieldClipboard?.Copy(field);
            }
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            var field = FieldClipboard?.Paste();
            if (field != null)
            {
                field = (FieldDisplayDetail) field.Clone();

                FieldListItemVO item = new()
                {
                    Id = field.Id,
                    FieldKey = field.FieldKey
                };
                fieldVOs.Add(item);
                lstFields.SelectedItem = item;

                fields.Add(field);
            }
        }

        private void BtnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldListItemVO selectedItem)
            {
                var srcIndex = fieldVOs.IndexOf(selectedItem);
                var dstIndex = (srcIndex - 1 + fieldVOs.Count) % fieldVOs.Count;

                fieldVOs.RemoveAt(srcIndex);
                fieldVOs.Insert(dstIndex, selectedItem);

                lstFields.SelectedItem = selectedItem;

                var field = fields[srcIndex];
                fields.RemoveAt(srcIndex);
                fields.Insert(dstIndex, field);
            }
        }

        private void BtnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            if (lstFields.SelectedItem is FieldListItemVO selectedItem)
            {
                var srcIndex = fieldVOs.IndexOf(selectedItem);
                var dstIndex = (srcIndex + 1) % fieldVOs.Count;

                fieldVOs.RemoveAt(srcIndex);
                fieldVOs.Insert(dstIndex, selectedItem);

                lstFields.SelectedItem = selectedItem;

                var field = fields[srcIndex];
                fields.RemoveAt(srcIndex);
                fields.Insert(dstIndex, field);
            }
        }

        public IEnumerable<FieldDisplayDetail> Fields
        {
            get
            {
                return new ReadOnlyCollection<FieldDisplayDetail>(fields.ToArray());
            }

            set
            {
                fields.Clear();
                fieldVOs.Clear();
                if (value == null)
                {
                    return;
                }

                fields.AddRange(value);
                value.Select(field => new FieldListItemVO()
                {
                    FieldKey = field.FieldKey,
                }).ToList()
                .ForEach(fieldVO => fieldVOs.Add(fieldVO));
            }
        }

        public ItemClipboard<FieldDisplayDetail> FieldClipboard
        {
            get; set;
        }
    }
}
