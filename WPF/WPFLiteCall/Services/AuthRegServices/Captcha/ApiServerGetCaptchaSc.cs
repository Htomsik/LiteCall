using System.Threading.Tasks;
using System.Windows.Media;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Captcha;

internal sealed class ApiServerGetCaptchaSc : IGetCaptchaSc
{
    private readonly CurrentServerStore _currentServerStore;


    private readonly IHttpDataSc _httpDataSc;

    private readonly IImageServices _imageServices;


    public ApiServerGetCaptchaSc(IHttpDataSc httpDataSc, IImageServices imageServices,
        CurrentServerStore currentServerStore)
    {
        _httpDataSc = httpDataSc;

        _imageServices = imageServices;

        _currentServerStore = currentServerStore;
    }


    public async Task<byte[]?> GetCaptcha()
    {
        var receiveBytes = await _httpDataSc.GetCaptcha(_currentServerStore.CurrentServer!.ApiIp);

        return _imageServices.GetRawData(receiveBytes) ?? null;
        
    }
}