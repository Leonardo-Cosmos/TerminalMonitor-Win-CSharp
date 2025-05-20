/* 2021/5/23 */
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerminalMonitor.Matchers;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FieldConditionView.xaml
    /// </summary>
    public partial class FieldConditionView : UserControl
    {
        public static readonly DependencyProperty FieldConditionProperty =
            DependencyProperty.Register(nameof(FieldCondition), typeof(FieldCondition), typeof(FieldConditionView),
                new PropertyMetadata(FieldCondition.Empty, OnFieldConditionChanged));

        private FieldCondition? fieldCondition;

        private readonly FieldConditionViewDataContextVO dataContextVO = new()
        {
            FieldKey = String.Empty,
            MatchOperator = TextMatchOperator.None,
            TargetValue = String.Empty,
        };

        public FieldConditionView()
        {
            InitializeComponent();

            cmbBxOperator.ItemsSource = Enum.GetValues(typeof(TextMatchOperator));
            stkPnl.DataContext = dataContextVO;
            dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
        }

        private void OnDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (fieldCondition == null)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(FieldConditionViewDataContextVO.FieldKey):
                    fieldCondition.FieldKey = dataContextVO.FieldKey;
                    break;
                case nameof(FieldConditionViewDataContextVO.MatchOperator):
                    fieldCondition.MatchOperator = dataContextVO.MatchOperator;
                    break;
                case nameof(FieldConditionViewDataContextVO.TargetValue):
                    fieldCondition.TargetValue = dataContextVO.TargetValue;
                    break;
                case nameof(FieldConditionViewDataContextVO.IsInverted):
                    fieldCondition.IsInverted = dataContextVO.IsInverted;
                    break;
                case nameof(FieldConditionViewDataContextVO.DefaultResult):
                    fieldCondition.DefaultResult = dataContextVO.DefaultResult;
                    break;
                case nameof(FieldConditionViewDataContextVO.IsDisabled):
                    fieldCondition.IsDisabled = dataContextVO.IsDisabled;
                    break;
                default:
                    break;
            }
        }

        private static void OnFieldConditionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var fieldConditionView = dependencyObject as FieldConditionView;
            fieldConditionView?.OnFieldConditionChanged(e);
        }

        private void OnFieldConditionChanged(DependencyPropertyChangedEventArgs e)
        {
            fieldCondition = e.NewValue as FieldCondition;
            if (fieldCondition != null)
            {
                dataContextVO.FieldKey = fieldCondition.FieldKey;
                dataContextVO.MatchOperator = fieldCondition.MatchOperator;
                dataContextVO.TargetValue = fieldCondition.TargetValue;

                dataContextVO.IsInverted = fieldCondition.IsInverted;
                dataContextVO.DefaultResult = fieldCondition.DefaultResult;
                dataContextVO.IsDisabled = fieldCondition.IsDisabled;
            }
        }

        public FieldCondition FieldCondition
        {
            get { return (FieldCondition)GetValue(FieldConditionProperty); }
            set { SetValue(FieldConditionProperty, value); }
        }

    }
}
