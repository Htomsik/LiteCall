using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services;

public class ImageServices : IImageServices
{
    public BitmapSource GetBitmapSource(Image streamImage)
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
}