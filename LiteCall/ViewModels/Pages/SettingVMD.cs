using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.ViewModels.Pages
{
    internal class SettingVMD:BaseVMD
    {

        private AuthorisationPageVMD _AuthorisationPageVMD;

        private RegistrationPageVMD _RegistrationPageVMD;


        private AccountStore _AccountStore;

        public AccountStore AccountStore
        {
            get => _AccountStore;
            set => Set(ref _AccountStore, value);
        }

        public SettingVMD(AccountStore accountStore, INavigationService AuthPagenavigationservices, INavigationService CloseAdditioNavigationService,AuthorisationPageVMD authorisationPageVmd, RegistrationPageVMD registrationPageVmd)
        {
           
            _AuthorisationPageVMD = authorisationPageVmd;

            _RegistrationPageVMD = registrationPageVmd;

            AccountStore = accountStore;

            OpenAuthCommand = new NavigationCommand(AuthPagenavigationservices);

            CloseSettingsCommand = new NavigationCommand(CloseAdditioNavigationService);

            AccountStore.CurrentAccountChange += AcoountStatusChange;

            AcoountStatusChange();


        }


        private BaseVMD _AccountCurrentVMD;

        public BaseVMD AccountCurrentVMD
        {
            get => _AccountCurrentVMD;
            set => Set(ref _AccountCurrentVMD, value);
        }

        public ICommand OpenAuthCommand { get; }

        public ICommand CloseSettingsCommand { get; }

        void AcoountStatusChange()
        {
            var IsDefault = AccountStore.isDefaulAccount;

            if (IsDefault)
            {

                AccountCurrentVMD = _AuthorisationPageVMD;

            }
            else
            {
                AccountCurrentVMD  = null;
            }
        }

     
    }
}
