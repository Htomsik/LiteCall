using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Services.Interfaces.Extra;

namespace Core.Services.AccountManagement.Captcha;

public sealed class MainServerGetCaptchaSc : IGetCaptchaSc
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