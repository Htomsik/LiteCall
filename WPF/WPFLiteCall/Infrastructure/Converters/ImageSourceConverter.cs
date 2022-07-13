using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using LiteCall.Services.Interfaces;

namespace LiteCall.Infrastructure.Converters;

public class ImageSourceConverter:IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return null;
        
        var captchaImage = BytesToImage((byte[]?)value);

       return GetBitmapSource(captchaImage);
       
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
    
    private BitmapSource? GetBitmapSource(Image streamImage)
    {
        var bitmap = new Bitmap(streamImage);

        var bmpPt = bitmap.GetHbitmap();

        var bitmapSource =
            Imaging.CreateBitmapSourceFromHBitmap(
                bmpPt,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

        bitmapSource.Freeze();

        return bitmapSource;
    }

    private Image BytesToImage(byte[]? value)
    {
        using var ms = new MemoryStream(value!);
        return Image.FromStream(ms);
    }
    
    
}