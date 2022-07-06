using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

public class UsernameValidator : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var stringArray = value.ToString()!.ToArray();

        if (stringArray.Length == 0) return new ValidationResult(true, null);

        var count = Regex.Matches(value.ToString()!, @"[!#@_|]", RegexOptions.IgnoreCase).Count;

        if (stringArray.Length < 4)
            return new ValidationResult(false, "Nickname can`t be less than 4");
        return count > 0
            ? new ValidationResult(false, "Nickname cannot contain the following characters:!#@_|")
            : new ValidationResult(true, null);
    }
}