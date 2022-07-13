using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class ApiServerRecoveryPasswordSc : IRecoveryPasswordSc
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