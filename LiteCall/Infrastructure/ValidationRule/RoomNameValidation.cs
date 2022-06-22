﻿using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

internal class RoomNameValidation : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var StringArray = value.ToString().ToArray();

        if (StringArray.Length == 0)
            return new ValidationResult(true, null);
        if (StringArray.Length < 3) return new ValidationResult(false, "RoomName can`t be less than 3");

        return new ValidationResult(true, null);
    }
}