using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters;

public class IsAuthorizeToStingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Authorized" : "Non authorized";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}