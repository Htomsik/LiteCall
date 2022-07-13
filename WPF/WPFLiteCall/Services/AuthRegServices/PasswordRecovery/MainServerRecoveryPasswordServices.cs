using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class MainServerRecoveryPasswordSc : IRecoveryPasswordSc
{
    private readonly IHttpDataSc _httpDataSc;

    public MainServerRecoveryPasswordSc(IHttpDataSc httpDataSc)
    {
        _httpDataSc = httpDataSc;
    }

    public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
    {
        return await _httpDataSc.PasswordRecovery(recoveryModel);
    }
}