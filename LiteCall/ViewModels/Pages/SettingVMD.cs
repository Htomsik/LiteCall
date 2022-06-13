using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.ViewModels.Pages
{
    internal class SettingVMD:BaseVMD
    {

        private readonly SettingsAccNavigationStore _SettingsAccNavigationStore;

        private readonly INavigationService _AuthNavigationService;




        private bool _IsDefault;

        public bool IsDefault
        {
            get => _IsDefault;
            set => Set(ref _IsDefault, value);
        }


        private AccountStore _AccountStore;

        public AccountStore AccountStore
        {
            get => _AccountStore;
            set => Set(ref _AccountStore, value);
        }


        private ServersAccountsStore _ServersAccountsStore;

        public ServersAccountsStore ServersAccountsStore
        {
            get => _ServersAccountsStore;
            set => Set(ref _ServersAccountsStore, value);
        }

        public SettingVMD(AccountStore accountStore,ServersAccountsStore serversAccountsStore, INavigationService CloseAdditioNavigationService, INavigationService AuthNavigationService ,SettingsAccNavigationStore SettingsAccNavigationStore)
        {

            AccountStore = accountStore;

            ServersAccountsStore = serversAccountsStore;

            
            _AuthNavigationService = AuthNavigationService;

            _SettingsAccNavigationStore = SettingsAccNavigationStore;

            LogoutAccCommand = new AccountLogoutCommand(_AccountStore);

            CloseSettingsCommand = new NavigationCommand(CloseAdditioNavigationService);

            AccountStore.CurrentAccountChange += AcoountStatusChange;

            _SettingsAccNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            AcoountStatusChange();


        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(AccountCurrentVMD));
        }

        public BaseVMD AccountCurrentVMD => _SettingsAccNavigationStore.SettingsAccCurrentViewModel;
        
        public ICommand LogoutAccCommand { get; }

        public ICommand CloseSettingsCommand { get; }


        void AcoountStatusChange()
        {

            IsDefault = AccountStore.isDefaultAccount;

            if (AccountStore.isDefaultAccount)
            {
                _AuthNavigationService.Navigate();

            }
            else
            {
                _SettingsAccNavigationStore.Close();
            }
           
        }

     
    }
}
