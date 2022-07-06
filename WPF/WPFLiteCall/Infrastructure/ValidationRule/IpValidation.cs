using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace LiteCall.Infrastructure.ValidationRule;

public class IpValidation : System.Windows.Controls.ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var stringValue = value.ToString();

        char[] splitValue = { '.', ':' };

        var splitArray = stringValue!.Split(splitValue).ToArray();

        if (splitArray.Length != 2)
        {
            if (splitArray.Length != 5) return new ValidationResult(false, "Incorrect Ip");
        }
        else
        {
            if (splitArray[0].ToLower() != "localhost") return new ValidationResult(false, "Incorrect Ip");
        }

        int port;

        try
        {
            port = Convert.ToInt32(splitArray[^1]);
        }
        catch
        {
            return new ValidationResult(false, "Incorrect Port");
        }

        if (port < 1 || port > 65536) return new ValidationResult(false, "Incorrect Port");

        splitArray = splitArray.SkipLast(1).ToArray();

        if (splitArray[0].ToLower() != "localhost")
            for (var i = 0; i < splitArray.Length; i++)
                if (Convert.ToInt32(i) < 0 || Convert.ToInt32(i) > 256)
                    return new ValidationResult(false, "Incorrect Ip");


        return new ValidationResult(true, null);
    }
}