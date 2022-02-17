/* 2021/6/23 */
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
using TerminalMonitor.Clipboard;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows
{
    /// <summary>
    /// Interaction logic for TerminalLineDetailWindow.xaml
    /// </summary>
    public partial class TerminalLineDetailWindow : Window
    {
        private TerminalLineDto terminalLineDto;

        public TerminalLineDetailWindow()
        {
            InitializeComponent();
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
                Condition groupCondition = new GroupCondition()
                {
                    Conditions = ConvertSelectedItemsToConditions(),
                };

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
                    Condition fieldCondition = new FieldCondition()
                    {
                        FieldKey = lineField.FieldKey,
                        MatchOperator = Matchers.TextMatchOperator.Equals,
                        TargetValue = lineField.Text,
                    };
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
