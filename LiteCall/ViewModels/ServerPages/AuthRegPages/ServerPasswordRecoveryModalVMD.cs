using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores.ModelStores;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerPasswordRecoveryModalVMD : PasswordRecoveryVMD
    {

        public ServerPasswordRecoveryModalVMD(INavigationService authPagenavigationservices,
            IStatusServices statusServices, 
            IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices,
            IRecoveryPasswordServices recoveryPasswordServices, IEncryptServices encryptServices) 
            : base(authPagenavigationservices, statusServices, getPassRecoveryQuestionsServices, recoveryPasswordServices, encryptServices)
        {


        }


    }
}
