using System.Threading.Tasks;
using System.Windows.Media;
using Core.Services.Interfaces.Connections;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Captcha;

internal sealed class MainServerGetCaptchaSc : IGetCaptchaSc
{
    private readonly IHttpDataSc _httpDataSc;
    private readonly IImageServices _imageServices;


    public MainServerGetCaptchaSc(IHttpDataSc httpDataSc, IImageServices imageServices)
    {
        _httpDataSc = httpDataSc;

        _imageServices = imageServices;
    }

    public async Task<byte[]?> GetCaptcha()
    {
        var receiveBytes = await _httpDataSc.GetCaptcha();

        return _imageServices.GetRawData(receiveBytes) ?? null;
    }
}