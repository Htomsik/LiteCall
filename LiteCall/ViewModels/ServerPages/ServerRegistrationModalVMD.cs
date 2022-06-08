using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerRegistrationModalVMD:RegistrationPageVMD
    {
        public ServerRegistrationModalVMD(AccountStore AccountStore, INavigationService MainPageNavigationServices, INavigationService AuthPagenavigationservices) : base(AccountStore, AuthPagenavigationservices)
        {

        }


        private bool _IsNotApiRegistration = false;
        public bool IsNotApiRegistration
        {
            get => _IsNotApiRegistration;
            set
            {
                Set(ref _IsNotApiRegistration, value);

            }
        }
    }
}
