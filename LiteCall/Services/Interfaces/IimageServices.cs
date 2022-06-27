using System.Drawing;
using System.Windows.Media.Imaging;

namespace LiteCall.Services.Interfaces;

public interface IImageServices
{
    public BitmapSource GetBitmapSource(Image streamImage);
}