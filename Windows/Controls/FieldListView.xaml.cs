/* 2021/5/12 */
using Microsoft.Toolkit.Mvvm.Input;
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
        private readonly FieldListViewDataContextVO dataContextVO;

        private readonly ObservableCollection<FieldListItemVO> fieldVOs = new();

        private List<FieldDisplayDetail> fields = new();

        private ItemClipboard<FieldDisplayDetail> fieldClipboard;

        public FieldListView()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                AddCommand = new RelayCommand(AddFieldDetail, () => !dataContextVO.IsAnySelected),
                RemoveCommand = new RelayCommand(RemoveSelectedFieldDetails, () => dataContextVO.IsAnySelected),
                EditCommand = new RelayCommand(EditSelectedFieldDetails, () => dataContextVO.IsAnySelected),
                MoveLeftCommand = new RelayCommand(MoveSelectedFieldDetailsLeft, () => dataContextVO.IsAnySelected),
                MoveRightCommand = new RelayCommand(MoveSelectedFieldDetailsRight, () => dataContextVO.IsAnySelected),
                CopyCommand = new RelayCommand(CopySelectedFieldDetails, () => dataContextVO.IsAnySelected),
                PasteCommnad = new RelayCommand(PasteFieldDetails, () => dataContextVO.IsAnyFieldInClipboard),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            lstFields.ItemsSource = fieldVOs;
        }

        private void DataContextVO_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FieldListViewDataContextVO.IsAnySelected):
                    (dataContextVO.AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveLeftCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveRightCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FieldListViewDataContextVO.IsAnyFieldInClipboard):
                    (dataContextVO.PasteCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstFields.SelectedItems.Count;
            dataContextVO.IsAnySelected = count > 0;
        }

        private void LstFields_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (hitResult.VisualHit.GetType() != typeof(ListBoxItem))
            {
                lstFields.UnselectAll();
            }
        }

        private void LstFields_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelectedFieldDetail();
        }

        private void FieldClipboard_ItemCopied(object sender, EventArgs e)
        {
            dataContextVO.IsAnyFieldInClipboard = fieldClipboard?.ContainsItem ?? false;
        }

        private void FieldClipboard_ItemPasted(object sender, EventArgs e)
        {
            dataContextVO.IsAnyFieldInClipboard = fieldClipboard?.ContainsItem ?? false;
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
                if (window.IsSaved)
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

        private void RemoveSelectedFieldDetails()
        {
            ForEachSelectedItem(RemoveFieldDetail);
        }

        private void RemoveFieldDetail(FieldListItemVO itemVO)
        {
            var index = fieldVOs.IndexOf(itemVO);
            fieldVOs.RemoveAt(index);

            fields.RemoveAt(index);
        }

        private void EditSelectedFieldDetail()
        {
            ForSelectedItem(EditFieldDetail);
        }

        private void EditSelectedFieldDetails()
        {
            ForEachSelectedItem(EditFieldDetail);
        }

        private void EditFieldDetail(FieldListItemVO itemVO)
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
                if (window.IsSaved)
                {
                    itemVO.FieldKey = fieldDetail.FieldKey;
                }
            };

            window.Show();
        }

        private void MoveSelectedFieldDetailsLeft()
        {
            ForEachSelectedItem(MoveFieldDetailLeft, byOrder: true, recoverSelection: true);
        }

        private void MoveFieldDetailLeft(FieldListItemVO itemVO)
        {
            var srcIndex = fieldVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex - 1 + fieldVOs.Count) % fieldVOs.Count;

            fieldVOs.RemoveAt(srcIndex);
            fieldVOs.Insert(dstIndex, itemVO);

            var fieldDetail = fields[srcIndex];
            fields.RemoveAt(srcIndex);
            fields.Insert(dstIndex, fieldDetail);
        }

        private void MoveSelectedFieldDetailsRight()
        {
            ForEachSelectedItem(MoveFieldDetailRight, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void MoveFieldDetailRight(FieldListItemVO itemVO)
        {
            var srcIndex = fieldVOs.IndexOf(itemVO);
            var dstIndex = (srcIndex + 1) % fieldVOs.Count;

            fieldVOs.RemoveAt(srcIndex);
            fieldVOs.Insert(dstIndex, itemVO);

            var fieldDetail = fields[srcIndex];
            fields.RemoveAt(srcIndex);
            fields.Insert(dstIndex, fieldDetail);
        }

        private void CopySelectedFieldDetails()
        {
            List<FieldDisplayDetail> copiedFieldDetails = new();
            foreach (var selectedItem in lstFields.SelectedItems)
            {
                if (selectedItem is FieldListItemVO itemVO)
                {
                    var index = fieldVOs.IndexOf(itemVO);

                    var fieldDetail = fields[index];
                    copiedFieldDetails.Add(fieldDetail);
                }
            }

            fieldClipboard?.Copy(copiedFieldDetails.ToArray());
        }

        private void PasteFieldDetails()
        {
            var pastedFieldDetails = fieldClipboard?.Paste();
            if (pastedFieldDetails != null)
            {
                lstFields.SelectedItems.Clear();
                foreach (var pastedFieldDetail in pastedFieldDetails)
                {
                    var fieldDetail = (FieldDisplayDetail)pastedFieldDetail.Clone();

                    FieldListItemVO itemVO = new()
                    {
                        Id = fieldDetail.Id,
                        FieldKey = fieldDetail.FieldKey
                    };
                    fieldVOs.Add(itemVO);
                    lstFields.SelectedItems.Add(itemVO);

                    fields.Add(fieldDetail);
                }
            }
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
