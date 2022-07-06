using Core.Services;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages.AutRegPasges;

namespace LiteCall.ViewModels.ServerPages.AuthRegPages;

internal sealed class ServerPasswordRecoveryModalVmd : PasswordRecoveryVmd
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