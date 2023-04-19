/* 2021/5/24 */
using Microsoft.Toolkit.Mvvm.Input;
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
using System.Windows.Shapes;
using TerminalMonitor.Clipboard;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using TerminalMonitor.Windows.ValidationRules;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for FieldDisplayDetailWindow.xaml
    /// </summary>
    public partial class FieldDisplayDetailWindow : Window
    {
        private readonly FieldDisplayDetailWindowDataContextVO dataContextVO;

        private readonly ObservableCollection<TextStyleCondition> styleConditions = new();

        private readonly List<string> existingFieldKeys = new();

        private FieldDisplayDetail fieldDetail;

        private ItemClipboard<TextStyleCondition> styleConditionClipboard;

        public FieldDisplayDetailWindow()
        {
            InitializeComponent();

            dataContextVO = new()
            {
                HeaderStyle = ColumnHeaderStyle.Empty,
                Style = TextStyle.Empty,

                AddCommand = new RelayCommand(AddCondition, () => true),
                RemoveCommand = new RelayCommand(RemoveSelectedConditions, () => dataContextVO.IsAnyConditionSelected),
                MoveUpCommand = new RelayCommand(MoveSelectedConditionsUp, () => dataContextVO.IsAnyConditionSelected),
                MoveDownCommand = new RelayCommand(MoveSelectedConditionsDown, () => dataContextVO.IsAnyConditionSelected),
                CutCommand = new RelayCommand(CutSelectedConditions,
                    () => dataContextVO.IsAnyConditionSelected && !dataContextVO.IsAnyConditionCutInClipboard),
                CopyCommand = new RelayCommand(CopySelectedConditions,
                    () => dataContextVO.IsAnyConditionSelected && !dataContextVO.IsAnyConditionCutInClipboard),
                PasteCommnad = new RelayCommand(PasteConditions, () => dataContextVO.IsAnyConditionInClipboard),
            };

            dataContextVO.PropertyChanged += DataContextVO_PropertyChanged;
            DataContext = dataContextVO;

            lstStyleCondtions.ItemsSource = styleConditions;

            Binding fieldKeyBinding = new("FieldKey");
            fieldKeyBinding.Source = dataContextVO;
            fieldKeyBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            fieldKeyBinding.ValidationRules.Add(new NotEmptyRule()
            {
                ErrorMessage = "Field key should not be empty",
            });
            txtBxKey.SetBinding(TextBox.TextProperty, fieldKeyBinding);
        }

        private void DataContextVO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FieldDisplayDetailWindowDataContextVO.IsAnyConditionSelected):
                    (dataContextVO.RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveUpCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.MoveDownCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FieldDisplayDetailWindowDataContextVO.IsAnyConditionInClipboard):
                    (dataContextVO.PasteCommnad as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                case nameof(FieldDisplayDetailWindowDataContextVO.IsAnyConditionCutInClipboard):
                    (dataContextVO.CutCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (dataContextVO.CopyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    break;
                default:
                    break;
            }
        }

        private void LstStyleCondtions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstStyleCondtions.SelectedItems.Count;
            dataContextVO.IsAnyConditionSelected = count > 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var fieldKeyHasError = Validation.GetHasError(txtBxKey);

            if (fieldKeyHasError)
            {
                txtBxKey.Focus();
                return;
            }

            SaveFieldDetail();
            IsSaved = true;
            Close();
        }

        private void ConditionClipboard_StatusChanged(object sender, EventArgs e)
        {
            UpdateClipboardStatus();
        }

        private void UpdateClipboardStatus()
        {
            dataContextVO.IsAnyConditionInClipboard = styleConditionClipboard.ContainsItem;
            dataContextVO.IsAnyConditionCutInClipboard = styleConditionClipboard.Status == ItemClipboardStatus.Move;
        }

        private void ForEachSelectedItem(Action<TextStyleCondition> action,
            bool byOrder = false, bool reverseOrder = false, bool recoverSelection = false)
        {
            List<TextStyleCondition> items = new();
            foreach (var selectedItem in lstStyleCondtions.SelectedItems)
            {
                if (selectedItem is TextStyleCondition item)
                {
                    items.Add(item);
                }
            }

            if (byOrder)
            {
                items.Sort((itemX, itemY) =>
                    styleConditions.IndexOf(itemX) - styleConditions.IndexOf(itemY));
            }

            if (reverseOrder)
            {
                items.Reverse();
            }

            items.ForEach(action);

            if (recoverSelection)
            {
                items.ForEach(item => lstStyleCondtions.SelectedItems.Add(item));
            }
        }

        private void InsertAtSelectedItem(params TextStyleCondition[] conditions)
        {
            var selectedIndex = lstStyleCondtions.SelectedIndex;
            if (selectedIndex == -1)
            {
                foreach (var condition in conditions)
                {
                    styleConditions.Add(condition);
                    lstStyleCondtions.SelectedItems.Add(condition);
                }
            }
            else
            {
                lstStyleCondtions.SelectedItems.Clear();

                var reversedConditions = conditions.Reverse().ToArray();
                foreach (var condition in reversedConditions)
                {
                    styleConditions.Insert(selectedIndex, condition);
                    lstStyleCondtions.SelectedItems.Add(condition);
                }
            }
        }

        private void AddCondition()
        {
            TextStyleCondition item = new()
            {
                Style = TextStyle.Empty,
                Condition = FieldCondition.Empty,
            };

            InsertAtSelectedItem(item);
        }

        private void RemoveSelectedConditions()
        {
            ForEachSelectedItem(RemoveCondition);
        }

        private void RemoveCondition(TextStyleCondition condition)
        {
            styleConditions.Remove(condition);
        }

        private void MoveSelectedConditionsUp()
        {
            ForEachSelectedItem(MoveConditionUp, byOrder: true, recoverSelection: true);
        }

        private void MoveConditionUp(TextStyleCondition condition)
        {
            var srcIndex = styleConditions.IndexOf(condition);
            var dstIndex = (srcIndex - 1 + styleConditions.Count) % styleConditions.Count;

            styleConditions.RemoveAt(srcIndex);
            styleConditions.Insert(dstIndex, condition);
        }

        private void MoveSelectedConditionsDown()
        {
            ForEachSelectedItem(MoveConditionDown, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void MoveConditionDown(TextStyleCondition condition)
        {
            var srcIndex = styleConditions.IndexOf(condition);
            var dstIndex = (srcIndex + 1) % styleConditions.Count;

            styleConditions.RemoveAt(srcIndex);
            styleConditions.Insert(dstIndex, condition);
        }

        private void CutSelectedConditions()
        {
            if (styleConditionClipboard != null)
            {
                List<TextStyleCondition> cutConditions = new();
                foreach (var selectedItem in lstStyleCondtions.SelectedItems)
                {
                    if (selectedItem is TextStyleCondition condition)
                    {
                        cutConditions.Add(condition);
                    }
                }

                styleConditionClipboard.Cut(cutConditions.ToArray());
                RemoveSelectedConditions();
            }
        }

        private void CopySelectedConditions()
        {
            if (styleConditionClipboard != null)
            {
                List<TextStyleCondition> copiedConditions = new();
                foreach (var selectedItem in lstStyleCondtions.SelectedItems)
                {
                    if (selectedItem is TextStyleCondition condition)
                    {
                        copiedConditions.Add(condition);
                    }
                }

                styleConditionClipboard.Copy(copiedConditions.ToArray());
            }
        }

        private void PasteConditions()
        {
            if (styleConditionClipboard != null)
            {
                (var pastedConditions, var clipboardStatus) = styleConditionClipboard.Paste();

                if (pastedConditions != null)
                {
                    var conditions = pastedConditions.Select(pastedCondition =>
                    {
                        var condition = clipboardStatus == ItemClipboardStatus.Move ?
                            pastedCondition : (TextStyleCondition)pastedCondition.Clone();

                        return condition;
                    }).ToArray();

                    InsertAtSelectedItem(conditions);
                }
            }
        }

        private void LoadFieldDetail(FieldDisplayDetail fieldDetail)
        {
            this.fieldDetail = fieldDetail;
            IsSaved = false;

            if (fieldDetail != null)
            {
                dataContextVO.FieldKey = fieldDetail.FieldKey;
                dataContextVO.HeaderName = fieldDetail.HeaderName;

                dataContextVO.CustomizeHeaderStyle = fieldDetail.CustomizeHeader;
                dataContextVO.HeaderStyle = fieldDetail.HeaderStyle ?? ColumnHeaderStyle.Empty;

                dataContextVO.CustomizeStyle = fieldDetail.CustomizeStyle;
                dataContextVO.Style = fieldDetail.Style ?? TextStyle.Empty;

                styleConditions.Clear();
                foreach (var condition in fieldDetail.Conditions ?? Array.Empty<TextStyleCondition>())
                {
                    styleConditions.Add(condition);
                }
            }
        }

        private void SaveFieldDetail()
        {
            if (fieldDetail != null)
            {
                fieldDetail.FieldKey = dataContextVO.FieldKey;
                fieldDetail.HeaderName = dataContextVO.HeaderName;
                fieldDetail.CustomizeHeader = dataContextVO.CustomizeHeaderStyle;
                fieldDetail.HeaderStyle = dataContextVO.HeaderStyle;
                fieldDetail.CustomizeStyle = dataContextVO.CustomizeStyle;
                fieldDetail.Style = dataContextVO.Style;
                fieldDetail.Conditions = styleConditions.ToArray();
            }
            else
            {
                fieldDetail = new FieldDisplayDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    FieldKey = dataContextVO.FieldKey,
                    HeaderName = dataContextVO.HeaderName,
                    CustomizeHeader = dataContextVO.CustomizeHeaderStyle,
                    HeaderStyle = dataContextVO.HeaderStyle,
                    CustomizeStyle = dataContextVO.CustomizeStyle,
                    Style = dataContextVO.Style,
                    Conditions = styleConditions.ToArray(),
                };
            }

            if (String.IsNullOrEmpty(fieldDetail.HeaderName))
            {
                fieldDetail.HeaderName = null;
            }
        }

        public bool IsSaved { get; set; }

        public IEnumerable<string> ExistingFieldKeys
        {
            get
            {
                return new ReadOnlyCollection<string>(existingFieldKeys.ToArray());
            }

            set
            {
                existingFieldKeys.Clear();
                if (value != null)
                {
                    existingFieldKeys.AddRange(value);
                }
            }
        }

        public FieldDisplayDetail FieldDetail
        {
            get => fieldDetail;
            set => LoadFieldDetail(value);
        }

        public ItemClipboard<TextStyleCondition> StyleConditionClipboard
        {
            get => styleConditionClipboard;

            set
            {
                if (styleConditionClipboard == value)
                {
                    return;
                }

                if (styleConditionClipboard != null)
                {
                    styleConditionClipboard.ItemCut -= ConditionClipboard_StatusChanged;
                    styleConditionClipboard.ItemCopied -= ConditionClipboard_StatusChanged;
                    styleConditionClipboard.ItemPasted -= ConditionClipboard_StatusChanged;
                }

                styleConditionClipboard = value;

                if (styleConditionClipboard != null)
                {
                    styleConditionClipboard.ItemCut += ConditionClipboard_StatusChanged;
                    styleConditionClipboard.ItemCopied += ConditionClipboard_StatusChanged;
                    styleConditionClipboard.ItemPasted += ConditionClipboard_StatusChanged;

                    UpdateClipboardStatus();
                }
            }
        }
    }
}
