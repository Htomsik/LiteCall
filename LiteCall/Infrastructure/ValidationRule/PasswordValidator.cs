using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule
{
    public class PasswordValidator : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var StringArray = value.ToString().ToArray();


            if (StringArray.Length == 0)
            {
                return new ValidationResult(true, null);
            }

            var HaveAnyBigLetter = StringArray.Any(item => char.IsUpper(item));

            if (StringArray.Length < 6)
            {
                return new ValidationResult(false, "Password can`t be less than 6 ");
            }
            else if (!HaveAnyBigLetter)
            {
                return new ValidationResult(false, @"Password must contain at least 1 capital letter");
            }


            return new ValidationResult(true, null);
        }
    }
}
