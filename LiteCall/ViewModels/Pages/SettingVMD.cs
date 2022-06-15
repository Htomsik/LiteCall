using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;

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

            AddNewServerCommand = new AsyncLamdaCommand(OnAddNewServerExecuted, (ex) => StatusMessage = ex.Message,
                CanAddNewServerExecute);


            inputDevice = new ObservableCollection<string>();

            outputDevice = new ObservableCollection<string>();

            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var capabilities = WaveIn.GetCapabilities(n);
                inputDevice.Add(capabilities.ProductName);
            }

            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                var capabilities = WaveIn.GetCapabilities(n);
                outputDevice.Add(capabilities.ProductName);
            }


        }



        private ObservableCollection<string> _inputDevice;

        public ObservableCollection<string> inputDevice
        {
            get => _inputDevice;
            set => Set(ref _inputDevice, value);
        }


        private ObservableCollection<string> _outputDevice;

        public ObservableCollection<string> outputDevice
        {
            get => _outputDevice;
            set => Set(ref _outputDevice, value);
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

        public ICommand AddNewServerCommand { get; }

        private bool CanAddNewServerExecute(object p) => !string.IsNullOrEmpty(NewServerApiIp) && !string.IsNullOrEmpty(NewSeverLogin);

        private async Task OnAddNewServerExecuted(object p)
        {

            var newSavedSeverAccount = new ServerAccount
            {
               Account = new Account{Login = NewSeverLogin},
               SavedServer = new Server
               {
                   ApiIp = NewServerApiIp
               }
            };

            if ( await DataBaseService.CheckServerStatus(NewServerApiIp))
            {
                if (!ServersAccountsStore.add(newSavedSeverAccount))
                {
                    MessageBox.Show("this server already saved", "Сообщение");
                }

            }
          

           

        }


        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                Set(ref _statusMessage, value);
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        private string _NewServerApiIp;

        public string NewServerApiIp
        {
            get => _NewServerApiIp;
            set => Set(ref _NewServerApiIp, value);
        }


        private string _NewSeverLogin;

        public string NewSeverLogin
        {
            get => _NewSeverLogin;
            set => Set(ref _NewSeverLogin, value);
        }





    }
}
