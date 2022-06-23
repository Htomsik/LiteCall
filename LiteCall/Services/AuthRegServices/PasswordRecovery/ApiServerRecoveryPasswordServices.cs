using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal class ApiServerRecoveryPasswordServices : IRecoveryPasswordServices
{
    private readonly CurrentServerStore _currentServerStore;
    private readonly IHttpDataServices _httpDataServices;


    public ApiServerRecoveryPasswordServices(IHttpDataServices httpDataServices, CurrentServerStore currentServerStore)
    {
        _httpDataServices = httpDataServices;
        _currentServerStore = currentServerStore;
    }

    public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
    {
        return await _httpDataServices.PasswordRecovery(recoveryModel, _currentServerStore.CurrentServer!.ApiIp);
    }
}