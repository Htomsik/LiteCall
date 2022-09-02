using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.String;

/// <summary>
///     True - Authorized
///     False - Non authorized
/// </summary>
public class IsAuthorizeBoolToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
        (bool)value ? "Authorized" : "Non authorized";
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(IsAuthorizeBoolToStringConverter)}.{nameof(ConvertBack)}");
}