using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services
{
    public class ImageServices:IimageServices
    {
        public  BitmapSource GetImageStream(System.Drawing.Image streamImage)
        {
            var bitmap = new Bitmap(streamImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpPt,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            bitmapSource.Freeze();
            
            return bitmapSource;
        }
    }
}
