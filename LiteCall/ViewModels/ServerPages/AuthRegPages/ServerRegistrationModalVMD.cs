 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerRegistrationModalVMD:RegistrationPageVMD
    {
        public ServerRegistrationModalVMD(INavigationService authPagenavigationservices,
            IRegistrationSevices registrationSevices,
            IStatusServices statusServices,
            ICaptchaServices captchaServices,
            IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices) 
            : base(authPagenavigationservices, registrationSevices, statusServices, captchaServices, getPassRecoveryQuestionsServices)
        {

          

        }


    }
}
