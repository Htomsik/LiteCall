using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.Null;


/// <summary>
///     Not null value -> True
///     Null -> False
/// </summary>
internal sealed class NullToBoolInverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) => value is not null;
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(NullToBoolInverter)}.{nameof(ConvertBack)}");
    
}