﻿using System;
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

        private readonly SettingsAccNavigationStore _SettingsAccNavigationStore;

        private readonly INavigationService _AuthNavigationService;


        private AccountStore _AccountStore;

        public AccountStore AccountStore
        {
            get => _AccountStore;
            set => Set(ref _AccountStore, value);
        }


        public SettingVMD(AccountStore accountStore, INavigationService CloseAdditioNavigationService, INavigationService AuthNavigationService ,SettingsAccNavigationStore SettingsAccNavigationStore)
        {

            _AuthNavigationService = AuthNavigationService;

            _SettingsAccNavigationStore = SettingsAccNavigationStore;

            AccountStore = accountStore;


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
            var IsDefault = AccountStore.isDefaulAccount;

            if (IsDefault)
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