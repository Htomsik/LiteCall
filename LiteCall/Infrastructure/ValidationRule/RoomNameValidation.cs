using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

internal sealed class RoomNameValidation : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var stringArray = value.ToString()!.ToArray();

        if (stringArray.Length == 0)
            return new ValidationResult(true, null);
        return stringArray.Length < 3
            ? new ValidationResult(false, "RoomName can`t be less than 3")
            : new ValidationResult(true, null);
    }
}