using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores.ModelStores;

namespace LiteCall.Services
{
    internal class ApiServerCaptchaServices:ICaptchaServices
    {


        private readonly IhttpDataServices _httpDataServices;

        private readonly IimageServices _imageServices;

        private readonly CurrentServerStore _currentServerStore;


        public ApiServerCaptchaServices(IhttpDataServices httpDataServices, IimageServices imageServices,CurrentServerStore currentServerStore)
        {
            _httpDataServices = httpDataServices;

            _imageServices = imageServices;

            _currentServerStore = currentServerStore;
        }


        public async Task<ImageSource?> GetCaptcha()
        {
            var receiveBytes = await _httpDataServices.GetCaptcha(_currentServerStore.CurrentServer.ApiIp);

            if (receiveBytes != null)
            {
                var captchaFromServer = ImageBox.BytesToImage(receiveBytes.GetRawData());

                var Capthca = _imageServices.GetImageStream(captchaFromServer);

                return Capthca;
            }

            return null;
        }
    }
}
