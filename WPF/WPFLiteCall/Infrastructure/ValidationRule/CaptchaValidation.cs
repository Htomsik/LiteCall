using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

public class CaptchaValidation : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var stringArray = value.ToString()!.ToArray();

        return stringArray.Length != 4
            ? new ValidationResult(false, "Captcha can`t be less or more than 4 ")
            : new ValidationResult(true, null);
    }
}