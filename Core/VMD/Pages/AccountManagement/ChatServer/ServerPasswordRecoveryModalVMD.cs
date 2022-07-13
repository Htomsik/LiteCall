using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;

namespace Core.VMD.Pages.AccountManagement.ChatServer;

public sealed class ServerPasswordRecoveryModalVmd : PasswordRecoveryVmd
{
    public ServerPasswordRecoveryModalVmd(INavigationSc authPageNavigationScs,
        IStatusSc statusSc,
        IGetRecoveryQuestionsSc getRecoveryQuestionsSc,
        IRecoveryPasswordSc recoveryPasswordSc, IEncryptSc encryptSc)
        : base(authPageNavigationScs, statusSc, getRecoveryQuestionsSc, recoveryPasswordSc,
            encryptSc)
    {
    }
}