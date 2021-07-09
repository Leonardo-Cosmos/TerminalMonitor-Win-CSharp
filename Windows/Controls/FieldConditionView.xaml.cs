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
            DependencyProperty.Register("FieldCondition", typeof(FieldCondition), typeof(FieldConditionView),
                new PropertyMetadata(FieldCondition.Empty, OnFieldConditionChanged));

        private FieldCondition fieldCondition;

        private readonly FieldConditionViewDataContextVO dataContextVO = new();

        public FieldConditionView()
        {
            InitializeComponent();

            cmbBxOperator.ItemsSource = Enum.GetValues(typeof(TextMatcher.MatchOperator));
            stkPnl.DataContext = dataContextVO;
            dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FieldKey":
                    fieldCondition.FieldKey = dataContextVO.FieldKey;
                    break;
                case "MatchOperator":
                    fieldCondition.MatchOperator = dataContextVO.MatchOperator;
                    break;
                case "TargetValue":
                    fieldCondition.TargetValue = dataContextVO.TargetValue;
                    break;
                default:
                    break;
            }
        }

        private static void OnFieldConditionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var fieldConditionView = dependencyObject as FieldConditionView;
            fieldConditionView.OnFieldConditionChanged(e);
        }

        private void OnFieldConditionChanged(DependencyPropertyChangedEventArgs e)
        {
            fieldCondition = e.NewValue as FieldCondition;
            if (fieldCondition != null)
            {
                dataContextVO.FieldKey = fieldCondition.FieldKey;
                dataContextVO.MatchOperator = fieldCondition.MatchOperator;
                dataContextVO.TargetValue = fieldCondition.TargetValue;
            }
        }

        public FieldCondition FieldCondition
        {
            get { return (FieldCondition)GetValue(FieldConditionProperty); }
            set { SetValue(FieldConditionProperty, value); }
        }

    }
}
