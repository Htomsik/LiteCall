using System.Globalization;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

public class AnswerValidation : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        return value.ToString()!.Length switch
        {
            0 => new ValidationResult(true, null),
            < 5 => new ValidationResult(false, "Answer can't less than 5"),
            _ => new ValidationResult(true, null)
        };
    }
}