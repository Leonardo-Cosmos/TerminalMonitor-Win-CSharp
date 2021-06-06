/* 2021/5/28 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TerminalMonitor.Windows.ValidationRules
{
    class UniqueItemRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var currentValue = value as string;

            if (ExistingValues?.Contains(currentValue) ?? false)
            {
                return new ValidationResult(false, ErrorMessage ?? "Value exists already");
            }

            return ValidationResult.ValidResult;
        }

        public IEnumerable<string> ExistingValues { get; init; }

        public string ErrorMessage { get; init; }
    }
}
