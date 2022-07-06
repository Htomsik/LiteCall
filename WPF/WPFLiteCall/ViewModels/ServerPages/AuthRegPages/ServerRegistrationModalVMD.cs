using Core.Services;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages.AutRegPasges;

namespace LiteCall.ViewModels.ServerPages.AuthRegPages;

internal sealed class ServerRegistrationModalVmd : RegistrationPageVmd
{
    public ServerRegistrationModalVmd(INavigationSc authPageNavigationScs,
        IRegistrationSc registrationSc,
        IStatusSc statusSc,
        IGetCaptchaSc getCaptchaSc,
        IGetRecoveryQuestionsSc getRecoveryQuestionsSc, IEncryptSc encryptSc)
        : base(authPageNavigationScs, registrationSc, statusSc, getCaptchaSc,
            getRecoveryQuestionsSc, encryptSc)
    {
    }
}