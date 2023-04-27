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

        private ItemClipboard<TextStyleCondition> styleConditionClipboard;

        public FieldListView()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                AddCommand = new RelayCommand(AddFieldDetail, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedFieldDetails, () => dataContextVO.IsAnyFieldSelected),
                EditCommand = new RelayCommand(EditSelectedFieldDetails, () => dataContextVO.IsAnyFieldSelected),
                MoveLeftCommand = new RelayCommand(MoveSelectedFieldDetailsLeft, () => dataContextVO.IsAnyFieldSelected),
                MoveRightCommand = new RelayCommand(MoveSelectedFieldDetailsRight, () => dataContextVO.IsAnyFieldSelected),
                CutCommand = new RelayCommand(CutSelectedFieldDetails,
                    () => dataContextVO.IsAnyFieldSelected && !dataContextVO.IsAnyFieldCutInClipboard),
                CopyCommand = new RelayCommand(CopySelectedFieldDetails,
                    () => dataContextVO.IsAnyFieldSelected && !dataContextVO.IsAnyFieldCutInClipboard),
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
                case nameof(FieldListViewDataContextVO.IsAnyFieldSelected):
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveLeftCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveRightCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FieldListViewDataContextVO.IsAnyFieldInClipboard):
                    (dataContextVO.PasteCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FieldListViewDataContextVO.IsAnyFieldCutInClipboard):
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstFields.SelectedItems.Count;
            dataContextVO.IsAnyFieldSelected = count > 0;
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

        private void FieldClipboard_StatusChanged(object sender, EventArgs e)
        {
            UpdateClipboardStatus();
        }

        private void UpdateClipboardStatus()
        {
            dataContextVO.IsAnyFieldInClipboard = fieldClipboard?.ContainsItem ?? false;
            dataContextVO.IsAnyFieldCutInClipboard = fieldClipboard?.Status == ItemClipboardStatus.Move;
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

        private void InsertAtSelectedItem(params (FieldDisplayDetail fieldDetail, FieldListItemVO itemVO)[] fieldTuples)
        {
            var selectedIndex = lstFields.SelectedIndex;
            if (selectedIndex == -1)
            {
                foreach (var (fieldDetail, itemVO) in fieldTuples)
                {
                    fieldVOs.Add(itemVO);
                    lstFields.SelectedItems.Add(itemVO);

                    fields.Add(fieldDetail);
                }
            }
            else
            {
                lstFields.SelectedItems.Clear();

                var reversedFieldTuples = fieldTuples.Reverse().ToArray();
                foreach (var (fieldDetail, itemVO) in reversedFieldTuples)
                {
                    fieldVOs.Insert(selectedIndex, itemVO);
                    lstFields.SelectedItems.Add(itemVO);

                    fields.Insert(selectedIndex, fieldDetail);
                }
            }
        }

        private void AddFieldDetail()
        {
            var existingFieldKeys = fields
                    .Select(field => field.FieldKey);
            FieldDisplayDetailWindow window = new()
            {
                ExistingFieldKeys = existingFieldKeys,
                StyleConditionClipboard = styleConditionClipboard,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    var fieldDetail = window.FieldDetail;

                    FieldListItemVO itemVO = new()
                    {
                        Id = fieldDetail.Id,
                        FieldKey = fieldDetail.FieldKey,
                        Hidden = fieldDetail.Hidden,
                    };

                    InsertAtSelectedItem((fieldDetail, itemVO));
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
                StyleConditionClipboard = styleConditionClipboard,
            };

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (window.IsSaved)
                {
                    itemVO.FieldKey = fieldDetail.FieldKey;
                    itemVO.Hidden = fieldDetail.Hidden;
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

        private void CutSelectedFieldDetails()
        {
            if (fieldClipboard != null)
            {
                List<FieldDisplayDetail> cutFieldDetails = new();
                foreach (var selectedItem in lstFields.SelectedItems)
                {
                    if (selectedItem is FieldListItemVO itemVO)
                    {
                        var index = fieldVOs.IndexOf(itemVO);

                        var fieldDetail = fields[index];
                        cutFieldDetails.Add(fieldDetail);
                    }
                }

                fieldClipboard.Cut(cutFieldDetails.ToArray());
                RemoveSelectedFieldDetails();
            }
        }

        private void CopySelectedFieldDetails()
        {
            if (fieldClipboard != null)
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

                fieldClipboard.Copy(copiedFieldDetails.ToArray());
            }
        }

        private void PasteFieldDetails()
        {
            if (fieldClipboard != null)
            {
                (var pastedFieldDetails, var clipboardStatus) = fieldClipboard.Paste();

                if (pastedFieldDetails != null)
                {
                    var fieldTuples = pastedFieldDetails.Select(pastedFieldDetail =>
                    {
                        var fieldDetail = clipboardStatus == ItemClipboardStatus.Move ?
                            pastedFieldDetail : (FieldDisplayDetail)pastedFieldDetail.Clone();

                        FieldListItemVO itemVO = new()
                        {
                            Id = fieldDetail.Id,
                            FieldKey = fieldDetail.FieldKey,
                            Hidden = fieldDetail.Hidden,
                        };

                        return (fieldDetail, itemVO);
                    }).ToArray();

                    InsertAtSelectedItem(fieldTuples);
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
                fields.Select(fieldDetail => new FieldListItemVO()
                {
                    FieldKey = fieldDetail.FieldKey,
                    Hidden = fieldDetail.Hidden,
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
                    fieldClipboard.ItemCut -= FieldClipboard_StatusChanged;
                    fieldClipboard.ItemCopied -= FieldClipboard_StatusChanged;
                    fieldClipboard.ItemPasted -= FieldClipboard_StatusChanged;
                }

                fieldClipboard = value;

                if (fieldClipboard != null)
                {
                    fieldClipboard.ItemCut += FieldClipboard_StatusChanged;
                    fieldClipboard.ItemCopied += FieldClipboard_StatusChanged;
                    fieldClipboard.ItemPasted += FieldClipboard_StatusChanged;

                    UpdateClipboardStatus();
                }
            }
        }

        public ItemClipboard<TextStyleCondition> StyleConditionClipboard
        {
            get => styleConditionClipboard;
            set => styleConditionClipboard = value;
        }
    }
}
