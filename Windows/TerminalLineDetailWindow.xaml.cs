/* 2021/6/23 */
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TerminalMonitor.Clipboard;
using TerminalMonitor.Matchers;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using TerminalMonitor.Windows.Helpers;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for TerminalLineDetailWindow.xaml
    /// </summary>
    public partial class TerminalLineDetailWindow : Window
    {
        private TerminalLineDto terminalLineDto;

        private readonly GridView gridView;

        private readonly GridViewColumn keyColumn;

        private readonly GridViewColumn valueColumn;

        public TerminalLineDetailWindow()
        {
            InitializeComponent();

            gridView = lstFields.View as GridView;
            keyColumn = gridView.Columns[0];
            valueColumn = gridView.Columns[1];
        }

        private void LstFields_Loaded(object sender, RoutedEventArgs e)
        {
            // Adjust column width to make it fit to list view size.
            AdjustColumnWidth();
        }

        private void BtnCopyListItem_Click(object sender, RoutedEventArgs e)
        {
            if (ConditionListClipboard != null)
            {
                ConditionListClipboard.Copy(ConvertSelectedItemsToConditions());
            }
        }

        private void BtnCopyTreeFieldNode_Click(object sender, RoutedEventArgs e)
        {
            if (ConditionTreeClipboard != null)
            {
                ConditionTreeClipboard.Copy(ConvertSelectedItemsToConditions());
            }
        }

        private void BtnCopyTreeGroupNode_Click(object sender, RoutedEventArgs e)
        {
            if (ConditionTreeClipboard != null)
            {
                Condition groupCondition = new GroupCondition(
                    GroupMatchMode.All,
                    ConvertSelectedItemsToConditions());

                ConditionTreeClipboard.Copy(groupCondition);
            }
        }

        private List<Condition> ConvertSelectedItemsToConditions()
        {
            List<Condition> conditions = new();
            foreach (var selectedItem in lstFields.SelectedItems)
            {
                if (selectedItem is TerminalLineFieldDto lineField)
                {
                    Condition fieldCondition = new FieldCondition(
                        lineField.FieldKey,
                        TextMatchOperator.Equals,
                        lineField.Text);
                    conditions.Add(fieldCondition);
                }
            }

            return conditions;
        }

        private void SetTerminalLineFields(Dictionary<string, TerminalLineFieldDto> lineFieldsDict)
        {
            var fields = lineFieldsDict.OrderBy(kvPair => kvPair.Key)
                .Select(kvPair => kvPair.Value)
                .ToList();
            lstFields.ItemsSource = fields;
        }

        private void ScrollViewer_VerticalScrollBarVisibilityChanged(object sender, EventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                valueColumn.Width = lstFields.ActualWidth - keyColumn.ActualWidth - 30;
            }
        }

        private void AdjustColumnWidth()
        {
            PropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(
                ScrollViewer.ComputedVerticalScrollBarVisibilityProperty, typeof(ScrollViewer));

            ScrollViewer scrollViewer = VisualTreeHelpers.FindChildOfType<ScrollViewer>(lstFields);
            propertyDescriptor.AddValueChanged(scrollViewer, ScrollViewer_VerticalScrollBarVisibilityChanged);

            valueColumn.Width = lstFields.ActualWidth - keyColumn.ActualWidth - 10;

            Task.Delay(TimeSpan.FromMilliseconds(100))
                .ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        propertyDescriptor.RemoveValueChanged(scrollViewer, ScrollViewer_VerticalScrollBarVisibilityChanged);
                    });
                });
        }

        public TerminalLineDto TerminalLine
        {
            get => terminalLineDto;
            set
            {
                terminalLineDto = value;
                SetTerminalLineFields(terminalLineDto.LineFieldDict);
            }
        }

        public ItemClipboard<Condition> ConditionListClipboard
        {
            get;
            set;
        }

        public ItemClipboard<Condition> ConditionTreeClipboard
        {
            get;
            set;
        }
    }
}
