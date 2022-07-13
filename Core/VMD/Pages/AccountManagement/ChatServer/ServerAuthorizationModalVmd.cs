using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;

namespace Core.VMD.Pages.AccountManagement.ChatServer;

public sealed class ServerAuthorizationModalVmd : AuthorizationPageVmd
{
    public ServerAuthorizationModalVmd(INavigationSc registrationNavigationScs,
        INavigationSc passwordRecoveryNavigationSc, IAuthorizationSc authorizationSc,
        IEncryptSc encryptSc) : base(registrationNavigationScs, passwordRecoveryNavigationSc,
        authorizationSc, encryptSc)
    {
    }
}