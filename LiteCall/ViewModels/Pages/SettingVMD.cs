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

        private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

        private readonly IhttpDataServices _httpDataServices;

        private readonly INavigationService _authNavigationService;

        private readonly IStatusServices _statusServices;


        public SettingVMD(AccountStore accountStore,ServersAccountsStore serversAccountsStore, INavigationService authNavigationService, IhttpDataServices httpDataServices,IStatusServices statusServices, SettingsAccNavigationStore settingsAccNavigationStore)
        {

            AccountStore = accountStore;

            ServersAccountsStore = serversAccountsStore;

            _settingsAccNavigationStore = settingsAccNavigationStore;


            _authNavigationService = authNavigationService;

            _httpDataServices = httpDataServices;

            _statusServices = statusServices;

            LogoutAccCommand = new AccountLogoutCommand(accountStore);

            AccountStore.CurrentAccountChange += AcoountStatusChange;

            _settingsAccNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            AcoountStatusChange();

            AddNewServerCommand = new AsyncLamdaCommand(OnAddNewServerExecuted, (ex) => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
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

        public BaseVMD AccountCurrentVMD => _settingsAccNavigationStore.SettingsAccCurrentViewModel;
        
        public ICommand LogoutAccCommand { get; }

        void AcoountStatusChange()
        {

            IsDefault = AccountStore.isDefaultAccount;

            if (AccountStore.isDefaultAccount)
            {
                _authNavigationService.Navigate();

            }
            else
            {
                _settingsAccNavigationStore.Close();
            }
           
        }

        public ICommand AddNewServerCommand { get; }

        private bool CanAddNewServerExecute(object p) => !string.IsNullOrEmpty(NewServerApiIp) && !string.IsNullOrEmpty(NewSeverLogin);

        private async Task OnAddNewServerExecuted(object p)
        {

            var newSavedSeverAccount = new ServerAccount
            {
               Account = new Account{Login = NewSeverLogin},

            };


           var serverStatus = await Task.Run(() => _httpDataServices.CheckServerStatus(NewServerApiIp));

            if (serverStatus)
            {

               Server newServer = await _httpDataServices.ApiServerGetInfo(NewServerApiIp);

               if (newServer == null) return;

               newServer.ApiIp = NewServerApiIp;

               newSavedSeverAccount.SavedServer = newServer;

                if (!ServersAccountsStore.Add(newSavedSeverAccount))
                {
                  _statusServices.ChangeStatus(new StatusMessage{Message = "Server already exists", isError = true});
                }

            }
          

           

        }

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




        private bool _isDefault;

        public bool IsDefault
        {
            get => _isDefault;
            set => Set(ref _isDefault, value);
        }


        private AccountStore _accountStore;

        public AccountStore AccountStore
        {
            get => _accountStore;
            set => Set(ref _accountStore, value);
        }


        private ServersAccountsStore _serversAccountsStore;

        public ServersAccountsStore ServersAccountsStore
        {
            get => _serversAccountsStore;
            set => Set(ref _serversAccountsStore, value);
        }

    }
}
