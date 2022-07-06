using System.Threading.Tasks;
using System.Windows.Media;
using Core.Stores.TemporaryInfo;
using LiteCall.Model.Images;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Captcha;

internal sealed class ApiServerGetCaptchaSc : IGetCaptchaSc
{
    private readonly CurrentServerStore _currentServerStore;


    private readonly IHttpDataServices _httpDataServices;

    private readonly IImageServices _imageServices;


    public ApiServerGetCaptchaSc(IHttpDataServices httpDataServices, IImageServices imageServices,
        CurrentServerStore currentServerStore)
    {
        _httpDataServices = httpDataServices;

        _imageServices = imageServices;

        _currentServerStore = currentServerStore;
    }


    public async Task<ImageSource?> GetCaptcha()
    {
        var receiveBytes = await _httpDataServices.GetCaptcha(_currentServerStore.CurrentServer!.ApiIp);

        if (receiveBytes == null) return null;

        var captchaFromServer = ImageBox.BytesToImage(receiveBytes.GetRawData());

        var bitmapSource = _imageServices.GetBitmapSource(captchaFromServer);

        return bitmapSource;
    }
}