using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Extra;

namespace Core.VMD.Pages.AccountManagement.ChatServer;

public sealed class ServerAuthorizationModalVmd : AuthorizationPageVmd
{
    public ServerAuthorizationModalVmd(INavigationServices registrationNavigationServiceses,
        INavigationServices passwordRecoveryNavigationServices, IAuthorizationSc authorizationSc,
        IEncryptSc encryptSc) : base(registrationNavigationServiceses, passwordRecoveryNavigationServices,
        authorizationSc, encryptSc)
    {
    }
}