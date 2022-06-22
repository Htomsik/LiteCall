using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters;

public class LoginPassValidatConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var tuple = new Tuple<object, object>(values[0], values[1]);
        return tuple;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}