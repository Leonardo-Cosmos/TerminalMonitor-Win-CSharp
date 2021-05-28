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
    class FieldKeyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var fieldKey = value as string;

            if (ExistingFieldKeys?.Contains(fieldKey) ?? false)
            {
                return new ValidationResult(false, "Field key had been added");
            }

            return ValidationResult.ValidResult;
        }

        public IEnumerable<string> ExistingFieldKeys { get; init; }
    }
}
