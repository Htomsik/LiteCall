using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class MainServerRecoveryPasswordSc : IRecoveryPasswordSc
{
    private readonly IHttpDataServices _httpDataServices;

    public MainServerRecoveryPasswordSc(IHttpDataServices httpDataServices)
    {
        _httpDataServices = httpDataServices;
    }

    public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
    {
        return await _httpDataServices.PasswordRecovery(recoveryModel);
    }
}