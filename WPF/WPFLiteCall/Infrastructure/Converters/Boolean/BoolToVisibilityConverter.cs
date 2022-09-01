using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.Boolean;

/// <summary>
///     True - Visible
///     False - Collapsed
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(BoolToVisibilityConverter)}.{nameof(ConvertBack)}");

}