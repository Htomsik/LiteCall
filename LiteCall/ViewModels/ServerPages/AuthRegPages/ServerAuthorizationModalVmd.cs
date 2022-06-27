using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages;

internal sealed class ServerAuthorizationModalVmd : AuthorizationPageVmd
{
    public ServerAuthorizationModalVmd(INavigationService registrationNavigationServices,
        INavigationService passwordRecoveryNavigationService, IAuthorizationServices authorizationServices,
        IEncryptServices encryptServices) : base(registrationNavigationServices, passwordRecoveryNavigationService,
        authorizationServices, encryptServices)
    {
    }
}