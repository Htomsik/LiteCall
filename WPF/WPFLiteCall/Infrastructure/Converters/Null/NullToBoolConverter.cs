using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.Null;

/// <summary>
///     Null -> True
///     Not null -> False
/// </summary>
public class NullToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) => value is null;
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
        throw new NotSupportedException($"{nameof(NullToBoolConverter)}.{nameof(ConvertBack)}");
    
}