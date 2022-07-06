using Core.Services;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages.AutRegPasges;

namespace LiteCall.ViewModels.ServerPages.AuthRegPages;

internal sealed class ServerAuthorizationModalVmd : AuthorizationPageVmd
{
    public ServerAuthorizationModalVmd(INavigationSc registrationNavigationScs,
        INavigationSc passwordRecoveryNavigationSc, IAuthorizationSc authorizationSc,
        IEncryptSc encryptSc) : base(registrationNavigationScs, passwordRecoveryNavigationSc,
        authorizationSc, encryptSc)
    {
    }
}