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

namespace LiteCall.ViewModels.Pages
{
    internal class SettingVMD:BaseVMD
    {

        private AccountStore _AccountStore;

        public AccountStore AccountStore
        {
            get => _AccountStore;
            set => Set(ref _AccountStore, value);
        }

        public SettingVMD(AccountStore accountStore, INavigationService AuthPagenavigationservices, INavigationService CloseAdditioNavigationService)
        {
            AccountStore = accountStore;

            OpenAuthCommand = new NavigationCommand(AuthPagenavigationservices);

            CloseSettingsCommand = new NavigationCommand(CloseAdditioNavigationService);

        }

        public ICommand OpenAuthCommand { get; }

        public ICommand CloseSettingsCommand { get; }

     
    }
}
