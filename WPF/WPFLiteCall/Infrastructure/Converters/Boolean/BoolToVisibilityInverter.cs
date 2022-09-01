using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.Boolean;

/// <summary>
///     True -> Collapsed
///     False -> Visible
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityInverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (bool)value ? Visibility.Collapsed : Visibility.Visible;
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(BoolToVisibilityInverter)}.{nameof(ConvertBack)}");
}