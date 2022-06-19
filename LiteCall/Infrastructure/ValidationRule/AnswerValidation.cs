using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule
{
    public class AnswerValidation: System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value.ToString().Length < 3) return new ValidationResult(true, null);
           

            if (value.ToString().Length < 5) return new ValidationResult(false, "Answer can't less than 5");

            return new ValidationResult(true, null);
        }
    }
}
