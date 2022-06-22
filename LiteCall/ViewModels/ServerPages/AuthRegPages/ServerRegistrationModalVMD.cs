using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages; 

internal class ServerRegistrationModalVmd : RegistrationPageVmd
{
    public ServerRegistrationModalVmd(INavigationService authPageNavigationServices,
        IRegistrationServices registrationServices,
        IStatusServices statusServices,
        ICaptchaServices captchaServices,
        IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices, IEncryptServices encryptServices)
        : base(authPageNavigationServices, registrationServices, statusServices, captchaServices,
            getPassRecoveryQuestionsServices, encryptServices)
    {
    }
}