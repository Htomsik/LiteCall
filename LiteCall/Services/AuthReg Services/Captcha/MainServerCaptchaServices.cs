using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiteCall.Model;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services
{
    internal class MainServerCaptchaServices:ICaptchaServices
    {
        private readonly IhttpDataServices _httpDataServices;
        private readonly IimageServices _imageServices;


        public MainServerCaptchaServices(IhttpDataServices httpDataServices, IimageServices imageServices)
        {
            _httpDataServices = httpDataServices;

            _imageServices = imageServices;
        }

        public async Task<ImageSource?> GetCaptcha()
        {
            var receiveBytes = await _httpDataServices.GetCaptcha();

            if (receiveBytes != null)
            {
                var captchaFromServer = ImageBox.BytesToImage(receiveBytes.GetRawData());

               var  Capthca = _imageServices.GetImageStream(captchaFromServer);

               return Capthca;
            }

            return null;
        }
    }
}
