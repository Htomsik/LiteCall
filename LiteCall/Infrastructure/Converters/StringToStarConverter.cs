using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters;

internal sealed class StringToStarConverter : IValueConverter
{
    public static readonly IValueConverter Instance = new StringToStarConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var starCount = value.ToString()!.ToArray().Length;

        var starsString = "";

        return starsString.PadLeft(starCount, '*');
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}