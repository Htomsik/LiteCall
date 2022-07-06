using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

public class PasswordValidator : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var stringArray = value.ToString()!.ToArray();

        if (stringArray.Length == 0) return new ValidationResult(true, null);

        var haveAnyBigLetter = stringArray.Any(item => char.IsUpper(item));

        if (stringArray.Length < 6)
            return new ValidationResult(false, "Password can`t be less than 6 ");
        return !haveAnyBigLetter
            ? new ValidationResult(false, @"Password must contain at least 1 capital letter")
            : new ValidationResult(true, null);
    }
}