using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;

namespace Core.VMD.Pages.AccountManagement.ChatServer;

public sealed class ServerPasswordRecoveryModalVmd : PasswordRecoveryVmd
{
    public ServerPasswordRecoveryModalVmd(INavigationServices authPageNavigationServiceses,
        IStatusSc statusSc,
        IGetRecoveryQuestionsSc getRecoveryQuestionsSc,
        IRecoveryPasswordSc recoveryPasswordSc, IEncryptSc encryptSc)
        : base(authPageNavigationServiceses, statusSc, getRecoveryQuestionsSc, recoveryPasswordSc,
            encryptSc)
    {
    }
}