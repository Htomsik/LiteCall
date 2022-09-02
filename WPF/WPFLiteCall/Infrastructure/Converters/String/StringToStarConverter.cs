using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters.String;

/// <summary>
///     Some string -> *
/// </summary>
[ValueConversion(typeof(string), typeof(string))]
internal sealed class StringToStarConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var starCount = value.ToString()!.ToArray().Length;

        var starsString = "";

        return starsString.PadLeft(starCount, '*');
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    => throw new NotSupportedException($"{nameof(StringToStarConverter)}.{nameof(ConvertBack)}");
}