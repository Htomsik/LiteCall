using System.Threading.Tasks;
using System.Windows.Media;
using LiteCall.Model;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Captcha;

internal class MainServerCaptchaServices : ICaptchaServices
{
    private readonly IHttpDataServices _httpDataServices;
    private readonly IImageServices _imageServices;


    public MainServerCaptchaServices(IHttpDataServices httpDataServices, IImageServices imageServices)
    {
        _httpDataServices = httpDataServices;

        _imageServices = imageServices;
    }

    public async Task<ImageSource?> GetCaptcha()
    {
        var receiveBytes = await _httpDataServices.GetCaptcha();

        if (receiveBytes == null) return null;

        var captchaFromServer = ImageBox.BytesToImage(receiveBytes.GetRawData());

        var bitmapSource = _imageServices.GetBitmapSource(captchaFromServer);

        return bitmapSource;
    }
}