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

    public class CaptchaValidation : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var StringArray = value.ToString().ToArray();

            if (StringArray.Length != 4)
            {
                return new ValidationResult(false, "Captcha can`t be less or more than 4 ");
            }
            
            return new ValidationResult(true, null);
        }
    }
}

