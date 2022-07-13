using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.PasswordRecovery;

public sealed class ApiServerRecoveryPasswordSc : IRecoveryPasswordSc
{
    private readonly CurrentServerStore _currentServerStore;
    private readonly IHttpDataSc _httpDataSc;


    public ApiServerRecoveryPasswordSc(IHttpDataSc httpDataSc, CurrentServerStore currentServerStore)
    {
        _httpDataSc = httpDataSc;
        _currentServerStore = currentServerStore;
    }

    public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
    {
        return await _httpDataSc.PasswordRecovery(recoveryModel, _currentServerStore.CurrentServer!.ApiIp);
    }
}