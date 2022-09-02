using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;

namespace Core.VMD.Pages.AccountManagement.ChatServer;

public sealed class ServerRegistrationModalVmd : RegistrationPageVmd
{
    public ServerRegistrationModalVmd(INavigationServices authPageNavigationServiceses,
        IRegistrationSc registrationSc,
        IStatusSc statusSc,
        IGetCaptchaSc getCaptchaSc,
        IGetRecoveryQuestionsSc getRecoveryQuestionsSc, IEncryptSc encryptSc)
        : base(authPageNavigationServiceses, registrationSc, statusSc, getCaptchaSc,
            getRecoveryQuestionsSc, encryptSc)
    {
    }
}