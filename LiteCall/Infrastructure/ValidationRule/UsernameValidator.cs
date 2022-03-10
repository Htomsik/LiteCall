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
    public class UsernameValidator : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {


            var StringArray = value.ToString().ToArray();

            var Count = Regex.Matches(value.ToString(), @"[!#@_|]", RegexOptions.IgnoreCase).Count;

            if (StringArray.Length < 4)
            {
                return new ValidationResult(false, "Nickname can`t be less than 4 ");
            }
            else if (Count>0)
            {
                return new ValidationResult(false, @"Nickname cannot contain the following characters:!#@_|");
            }


            return new ValidationResult(true, null);
        }
    }
}