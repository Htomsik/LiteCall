using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.Boolean;

/// <summary>
///     Bool -> !bool
/// </summary>
[ValueConversion(typeof(bool), typeof(bool))]
public class BoolInverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
        throw new NotSupportedException($"{nameof(BoolInverter)}.{nameof(ConvertBack)}");
   
}