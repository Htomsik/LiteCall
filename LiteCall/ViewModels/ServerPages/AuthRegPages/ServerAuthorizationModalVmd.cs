using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages;

internal class ServerAuthorizationModalVmd : AuthorizationPageVmd
{
    public ServerAuthorizationModalVmd(INavigationService registrationNavigationServices,
        INavigationService passwordRecoveryNavigationService, IAuthorizationServices authorizationServices,
        IEncryptServices encryptServices) : base(registrationNavigationServices, passwordRecoveryNavigationService,
        authorizationServices, encryptServices)
    {
    }
}