using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LiteCall.Infrastructure.Converters.Image;

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
    
    private BitmapSource? GetBitmapSource(System.Drawing.Image streamImage)
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

    private System.Drawing.Image BytesToImage(byte[]? value)
    {
        using var ms = new MemoryStream(value!);
        return System.Drawing.Image.FromStream(ms);
    }
    
    
}