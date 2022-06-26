using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Model.RegistrationRecovery;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class MainServerRecoveryPasswordServices : IRecoveryPasswordServices
{
    private readonly IHttpDataServices _httpDataServices;

    public MainServerRecoveryPasswordServices(IHttpDataServices httpDataServices)
    {
        _httpDataServices = httpDataServices;
    }

    public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
    {
        return await _httpDataServices.PasswordRecovery(recoveryModel);
    }
}