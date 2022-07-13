using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;

namespace Core.VMD.Pages.AccountManagement.ChatServer;

public sealed class ServerRegistrationModalVmd : RegistrationPageVmd
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