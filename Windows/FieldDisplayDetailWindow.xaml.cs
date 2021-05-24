/* 2021/5/24 */
using System;
using System.Collections.Generic;
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
using TerminalMonitor.Models;

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

        public FieldDisplayDetailWindow()
        {
            InitializeComponent();

            DataContext = dataContextVO;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var conditions = dataContextVO.Conditions;
            TextStyleCondition item = new()
            {
                Style = TextStyle.Empty,
                Condition = TextCondition.Empty,
            };
            conditions.Add(item);
            lstStyleCondtions.SelectedItem = item;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var conditions = dataContextVO.Conditions;
            if (lstStyleCondtions.SelectedItem is TextStyleCondition selectedItem)
            {
                conditions.Remove(selectedItem);
            }
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var conditions = dataContextVO.Conditions;
            if (lstStyleCondtions.SelectedItem is TextStyleCondition selectedItem)
            {
                var index = conditions.IndexOf(selectedItem);
                if (index > 0)
                {
                    conditions.Remove(selectedItem);
                    conditions.Insert(index - 1, selectedItem);

                    lstStyleCondtions.SelectedItem = selectedItem;
                }
            }
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var conditions = dataContextVO.Conditions;
            if (lstStyleCondtions.SelectedItem is TextStyleCondition selectedItem)
            {
                var index = conditions.IndexOf(selectedItem);
                if (index < conditions.Count - 1)
                {
                    conditions.Remove(selectedItem);
                    conditions.Insert(index + 1, selectedItem);

                    lstStyleCondtions.SelectedItem = selectedItem;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public FieldDisplayDetail Field
        {
            get
            {
                return new FieldDisplayDetail()
                {
                    FieldKey = dataContextVO.FieldKey,
                    UseDefaultStyle = dataContextVO.UseDefaultStyle,
                    Style = dataContextVO.Style,
                    Conditions = dataContextVO.Conditions.ToArray(),
                };
            }
            set
            {
                if (value != null)
                {
                    dataContextVO.FieldKey = value.FieldKey;
                    dataContextVO.UseDefaultStyle = value.UseDefaultStyle;

                    dataContextVO.Style = value.Style ?? TextStyle.Empty;

                    dataContextVO.Conditions.Clear();
                    foreach(var condition in value.Conditions ?? Array.Empty<TextStyleCondition>())
                    {
                        dataContextVO.Conditions.Add(condition);
                    }
                }
            }
        }
    }
}
