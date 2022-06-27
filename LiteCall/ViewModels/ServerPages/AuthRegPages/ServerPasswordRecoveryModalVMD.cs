using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages;

internal sealed class ServerPasswordRecoveryModalVmd : PasswordRecoveryVmd
{
    public ServerPasswordRecoveryModalVmd(INavigationService authPageNavigationServices,
        IStatusServices statusServices,
        IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices,
        IRecoveryPasswordServices recoveryPasswordServices, IEncryptServices encryptServices)
        : base(authPageNavigationServices, statusServices, getPassRecoveryQuestionsServices, recoveryPasswordServices,
            encryptServices)
    {
    }
}