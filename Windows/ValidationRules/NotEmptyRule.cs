/* 2021/6/6 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TerminalMonitor.Windows.ValidationRules
{
    class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var currentValue = value as string;

            if (String.IsNullOrEmpty(currentValue))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return ValidationResult.ValidResult;
        }

        public required string ErrorMessage { get; set; }
    }
}
