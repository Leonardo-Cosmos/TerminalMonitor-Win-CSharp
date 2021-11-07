/* 2021/5/12 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly FieldListViewDataContextVO dataContextVO = new();

        private readonly ObservableCollection<FieldListItemVO> fieldVOs = new();

        private List<FieldDisplayDetail> fields = new();

        private ItemClipboard<FieldDisplayDetail> fieldClipboard;

        public FieldListView()
        {
            InitializeComponent();

            DataContext = dataContextVO;

            lstFields.ItemsSource = fieldVOs;
        }

        private void LstFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstFields.SelectedItems.Count;
            dataContextVO.IsAnySelected = count > 0;
        }

        private void LstFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ForSelectedItem(ModifyFieldDetail);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddFieldDetail();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(DeleteFieldDetail);
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(ModifyFieldDetail);
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            CopyFieldDetails();
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            PasteFieldDetails();
        }

        private void BtnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(MoveFieldDetailUp, byOrder: true, recoverSelection: true);
        }

        private void BtnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedItem(MoveFieldDetailDown, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void ForSelectedItem(Action<FieldListItemVO> action)
        {
            if (lstFields.SelectedItem is FieldListItemVO itemVO)
            {
                action(itemVO);
            }
        }

        private void ForEachSelectedItem(Action<FieldListItemVO> action,
            bool byOrder = false, bool reverseOrder = false, bool recoverSelection = false)
        {
            List<FieldListItemVO> itemVOs = new();
            foreach (var selectedItem in lstFields.SelectedItems)
            {
                if (selectedItem is FieldListItemVO itemVO)
                {
                    itemVOs.Add(itemVO);
                }
            }

            if (byOrder)
            {
                itemVOs.Sort((itemX, itemY) =>
                    fieldVOs.IndexOf(itemX) - fieldVOs.IndexOf(itemY));
            }

            if (reverseOrder)
            {
                itemVOs.Reverse();
            }

            itemVOs.ForEach(action);

            if (recoverSelection)
            {
                itemVOs.ForEach(itemVO => lstFields.SelectedItems.Add(itemVO));
            }
        }

        private void AddFieldDetail()
        {
            var existingFieldKeys = fields
                    .Select(field => field.FieldKey);
            FieldDisplayDetailWindow window = new()
            {
                ExistingFieldKeys = existingFieldKeys,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.Saved)
                {
                    var fieldDetail = window.FieldDetail;

                    FieldListItemVO item = new()
                    {
                        Id = fieldDetail.Id,
                        FieldKey = fieldDetail.FieldKey
                    };
                    fieldVOs.Add(item);
                    lstFields.SelectedItem = item;

                    fields.Add(fieldDetail);
                }
            };

            window.Show();
        }

        private void DeleteFieldDetail(FieldListItemVO itemVO)
        {
            var index = fieldVOs.IndexOf(itemVO);
            fieldVOs.RemoveAt(index);

            fields.RemoveAt(index);
        }

        private void ModifyFieldDetail(FieldListItemVO itemVO)
        {
            var index = fieldVOs.IndexOf(itemVO);

            var fieldDetail = fields[index];
            var existingFieldKeys = fields
                .Select(field => field.FieldKey)
                .Where(fieldKey => fieldKey != fieldDetail.FieldKey);
            FieldDisplayDetailWindow window = new()
            {
                FieldDetail = fieldDetail,
                ExistingFieldKeys = existingFieldKeys,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                itemVO.FieldKey = fieldDetail.FieldKey;
            };

            window.Show();
        }

        private void MoveFieldDetailUp(FieldListItemVO itemVO)
        {
            var srcIndex = fieldVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex - 1 + fieldVOs.Count) % fieldVOs.Count;

            fieldVOs.RemoveAt(srcIndex);
            fieldVOs.Insert(dstIndex, itemVO);

            var fieldDetail = fields[srcIndex];
            fields.RemoveAt(srcIndex);
            fields.Insert(dstIndex, fieldDetail);
        }

        private void MoveFieldDetailDown(FieldListItemVO itemVO)
        {
            var srcIndex = fieldVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex + 1) % fieldVOs.Count;

            fieldVOs.RemoveAt(srcIndex);
            fieldVOs.Insert(dstIndex, itemVO);

            var fieldDetail = fields[srcIndex];
            fields.RemoveAt(srcIndex);
            fields.Insert(dstIndex, fieldDetail);
        }

        private void CopyFieldDetails()
        {
            List<FieldDisplayDetail> fieldDetails = new();
            foreach (var selectedItem in lstFields.SelectedItems)
            {
                if (selectedItem is FieldListItemVO itemVO)
                {
                    var index = fieldVOs.IndexOf(itemVO);

                    var fieldDetail = fields[index];
                    fieldDetails.Add(fieldDetail);
                }
            }

            fieldClipboard?.Copy(fieldDetails.ToArray());
        }

        private void PasteFieldDetails()
        {
            var fieldDetails = fieldClipboard?.Paste();
            if (fieldDetails != null)
            {
                foreach (var fieldDetail in fieldDetails)
                {
                    var fieldDetailClone = (FieldDisplayDetail)fieldDetail.Clone();

                    FieldListItemVO item = new()
                    {
                        Id = fieldDetailClone.Id,
                        FieldKey = fieldDetailClone.FieldKey
                    };
                    fieldVOs.Add(item);
                    lstFields.SelectedItem = item;

                    fields.Add(fieldDetailClone);
                }
            }
        }

        private void FieldClipboard_ItemCopied(object sender, EventArgs e)
        {
            dataContextVO.IsAnyFieldInClipboard = !fieldClipboard?.IsEmpty ?? false;
        }

        private void FieldClipboard_ItemPasted(object sender, EventArgs e)
        {
            dataContextVO.IsAnyFieldInClipboard = !fieldClipboard?.IsEmpty ?? false;
        }

        public List<FieldDisplayDetail> Fields
        {
            get => fields;

            set
            {
                fields = value ?? new();

                fieldVOs.Clear();
                fields.Select(field => new FieldListItemVO()
                {
                    FieldKey = field.FieldKey,
                }).ToList()
                .ForEach(fieldVO => fieldVOs.Add(fieldVO));
            }
        }

        public ItemClipboard<FieldDisplayDetail> FieldClipboard
        {
            get => fieldClipboard;

            set
            {
                if (fieldClipboard == value)
                {
                    return;
                }

                if (fieldClipboard != null)
                {
                    fieldClipboard.ItemCopied -= FieldClipboard_ItemCopied;
                    fieldClipboard.ItemPasted -= FieldClipboard_ItemPasted;

                }

                fieldClipboard = value;

                if (fieldClipboard != null)
                {
                    fieldClipboard.ItemCopied += FieldClipboard_ItemCopied;
                    fieldClipboard.ItemPasted += FieldClipboard_ItemPasted;
                }
            }
        }
    }
}
