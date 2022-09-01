using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.String;

/// <summary>
///     Some string > 0 -> True
///     Else -> False
/// </summary>
[ValueConversion(typeof(string), typeof(bool))]
internal sealed class IsStringMoreThanZeroConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value.ToString()!.Length > 0;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(IsStringMoreThanZeroConverter)}.{nameof(ConvertBack)}");
}