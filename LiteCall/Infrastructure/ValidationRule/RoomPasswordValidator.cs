using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

public class RoomPasswordValidator : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var stringArray = value.ToString().ToArray();

        if (stringArray.Length == 0)
            return new ValidationResult(true, null);
        return stringArray.Length < 1 ? new ValidationResult(false, "Password can`t be less than 1") : new ValidationResult(true, null);
    }
}