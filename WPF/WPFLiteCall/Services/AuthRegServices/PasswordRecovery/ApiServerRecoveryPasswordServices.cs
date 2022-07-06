using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class ApiServerRecoveryPasswordSc : IRecoveryPasswordSc
{
    private readonly CurrentServerStore _currentServerStore;
    private readonly IHttpDataServices _httpDataServices;


    public ApiServerRecoveryPasswordSc(IHttpDataServices httpDataServices, CurrentServerStore currentServerStore)
    {
        _httpDataServices = httpDataServices;
        _currentServerStore = currentServerStore;
    }

    public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
    {
        return await _httpDataServices.PasswordRecovery(recoveryModel, _currentServerStore.CurrentServer!.ApiIp);
    }
}