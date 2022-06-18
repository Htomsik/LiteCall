using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LiteCall.Services.Interfaces
{
    public interface IimageServices
    {
        public BitmapSource GetImageStream(System.Drawing.Image streamImage);
    }
}
