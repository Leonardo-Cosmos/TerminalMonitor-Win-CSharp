/* 2021/5/24 */
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
        private readonly FieldDisplayDetailWindowDataContextVO dataContextVO = new()
        {
            Style = TextStyle.Empty,
        };

        private readonly List<string> existingFieldKeys = new();

        private FieldDisplayDetail fieldDetail;

        public FieldDisplayDetailWindow()
        {
            InitializeComponent();

            DataContext = dataContextVO;

            Binding fieldKeyBinding = new("FieldKey");
            fieldKeyBinding.Source = dataContextVO;
            fieldKeyBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            fieldKeyBinding.ValidationRules.Add(new NotEmptyRule()
            {
                ErrorMessage = "Field key should not be empty",
            });
            txtBxKey.SetBinding(TextBox.TextProperty, fieldKeyBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Saved = false;
        }

        private void LstStyleCondtions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = lstStyleCondtions.SelectedItems.Count;
            dataContextVO.IsAnyConditionSelected = count > 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCondition();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedCondition(DeleteCondition);
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedCondition(MoveConditionUp, byOrder: true, recoverSelection: true);
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            ForEachSelectedCondition(MoveConditionDown, byOrder: true, reverseOrder: true, recoverSelection: true);
        }

        private void ForEachSelectedCondition(Action<TextStyleCondition> action,
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
                var conditions = dataContextVO.Conditions;
                items.Sort((itemX, itemY) =>
                    conditions.IndexOf(itemX) - conditions.IndexOf(itemY));
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var fieldKeyHasError = Validation.GetHasError(txtBxKey);

            if (fieldKeyHasError)
            {
                txtBxKey.Focus();
                return;
            }

            SaveFieldDetail();
            Saved = true;
            Close();
        }

        private void AddCondition()
        {
            var conditions = dataContextVO.Conditions;
            TextStyleCondition item = new()
            {
                Style = TextStyle.Empty,
                Condition = FieldCondition.Empty,
            };
            conditions.Add(item);
            lstStyleCondtions.SelectedItem = item;
        }

        private void DeleteCondition(TextStyleCondition condition)
        {
            var conditions = dataContextVO.Conditions;
            conditions.Remove(condition);
        }

        private void MoveConditionUp(TextStyleCondition condition)
        {
            var conditions = dataContextVO.Conditions;

            var srcIndex = conditions.IndexOf(condition);
            var dstIndex = (srcIndex - 1 + conditions.Count) % conditions.Count;

            conditions.RemoveAt(srcIndex);
            conditions.Insert(dstIndex, condition);
        }

        private void MoveConditionDown(TextStyleCondition condition)
        {
            var conditions = dataContextVO.Conditions;

            var srcIndex = conditions.IndexOf(condition);
            var dstIndex = (srcIndex + 1) % conditions.Count;

            conditions.RemoveAt(srcIndex);
            conditions.Insert(dstIndex, condition);
        }

        private void LoadFieldDetail()
        {
            if (fieldDetail != null)
            {
                dataContextVO.FieldKey = fieldDetail.FieldKey;
                dataContextVO.CustomizeStyle = fieldDetail.CustomizeStyle;

                dataContextVO.Style = fieldDetail.Style ?? TextStyle.Empty;

                dataContextVO.Conditions.Clear();
                foreach (var condition in fieldDetail.Conditions ?? Array.Empty<TextStyleCondition>())
                {
                    dataContextVO.Conditions.Add(condition);
                }
            }
        }

        private void SaveFieldDetail()
        {
            if (fieldDetail != null)
            {
                fieldDetail.FieldKey = dataContextVO.FieldKey;
                fieldDetail.CustomizeStyle = dataContextVO.CustomizeStyle;
                fieldDetail.Style = dataContextVO.Style;
                fieldDetail.Conditions = dataContextVO.Conditions.ToArray();
            }
        }

        private FieldDisplayDetail CreateFieldDetail()
        {
            return new FieldDisplayDetail()
            {
                Id = Guid.NewGuid().ToString(),
                FieldKey = dataContextVO.FieldKey,
                CustomizeStyle = dataContextVO.CustomizeStyle,
                Style = dataContextVO.Style,
                Conditions = dataContextVO.Conditions.ToArray(),
            };
        }

        public bool Saved { get; set; }

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
            get => fieldDetail ?? CreateFieldDetail();

            set
            {
                fieldDetail = value;
                LoadFieldDetail();
            }
        }
    }
}
