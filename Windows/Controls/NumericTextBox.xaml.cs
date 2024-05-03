/* 2024/1/11 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : UserControl
    {
        private static readonly Regex digitRegex = new(@"\d+");

        private static readonly Regex numberRegex = new(@"^[1-9]\d*");

        public static readonly DependencyProperty valueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnNumberPropertyChanged));

        private int value;
        private readonly NumericTextBoxDataContextVO dataContextVO = new();

        private bool isPropertyChangedCallbackSuspended = false;

        public NumericTextBox()
        {
            InitializeComponent();

            txtBx.DataContext = dataContextVO;
        }

        private void TxtBx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValidInput(e.Text);
        }

        private static bool IsValidInput(string text)
        {
            return digitRegex.IsMatch(text);
        }

        private bool IsValidText(string text)
        {
            if (!numberRegex.IsMatch(text))
            {
                return false;
            }

            int number = Int32.Parse(text);
            return IsValidNumber(number);
            
        }

        private bool IsValidNumber(int number)
        {
            return number <= MaxValue && number >= MinValue;
        }

        private string GetValidText(string text)
        {
            if (!numberRegex.IsMatch(text))
            {
                return null;
            }

            int number = Int32.Parse(text);
            if (number > MaxValue)
            {
                return MaxValue.ToString();

            }
            else if (number < MinValue)
            {
                return MinValue.ToString();
            }
            else
            {
                return null;
            }
        }

        private void TxtBx_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsValidInput(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NumericTextBoxDataContextVO.NumberText):
                    var numberText = dataContextVO.NumberText;
                    if (IsValidText(numberText))
                    {
                        isPropertyChangedCallbackSuspended = true;
                        Value = Int32.Parse(numberText);
                        isPropertyChangedCallbackSuspended = false;
                    }
                    else
                    {
                        dataContextVO.PropertyChanged -= OnDataContextPropertyChanged;
                        dataContextVO.NumberText = GetValidText(numberText) ?? value.ToString();
                        dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
                    }
                    break;

                default:
                    break;
            }
        }

        private static void OnNumberPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var numericTextBox = dependencyObject as NumericTextBox;
            numericTextBox.OnNumberPropertyChanged(e);
        }

        private void OnNumberPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (isPropertyChangedCallbackSuspended)
            {
                return;
            }
            value = (e.NewValue as int?) ?? 0;
            if (IsValidNumber(value))
            {
                dataContextVO.PropertyChanged -= OnDataContextPropertyChanged;
                dataContextVO.NumberText = value.ToString();
                dataContextVO.PropertyChanged += OnDataContextPropertyChanged;
            }
        }

        public int Value
        {
            get => (int)GetValue(valueProperty);
            set => SetValue(valueProperty, value);
        }

        public int MaxValue { get; set; } = 100;

        public int MinValue { get; set; } = 0;
    }
}
