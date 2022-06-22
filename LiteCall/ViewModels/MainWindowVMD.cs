using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Base;
using Microsoft.Extensions.Configuration;

namespace LiteCall.ViewModels
{   
    internal class MainWindowVMD:BaseVmd
    {

        

        public string Version => _configuration!.GetSection("AppSettings")["AppVersions"] ?? "0.1.0";

        public string Vetka => _configuration!.GetSection("AppSettings")["Branch"] ?? "NonStable";


        public MainWindowVMD(MainWindowNavigationStore mainWindowNavigationStore, 
            AdditionalNavigationStore additionalNavigationStore,
            ModalNavigationStore modalNavigationStore,
            StatusMessageStore statusMessageStore,
            INavigationService closeModalNavigationServices,
            INavigationService CloseAdditioNavigationService,
            IStatusServices statusServices,
            ICloseAppServices closeAppServices,IConfiguration configuration)
        {
            

            _mainWindowNavigationStore = mainWindowNavigationStore;

            _additionalNavigationStore = additionalNavigationStore;

            _modalNavigationStore = modalNavigationStore;

            _statusMessageStore = statusMessageStore;

            _closeAppServices = closeAppServices;

            _configuration = configuration;

            _mainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _additionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;

            _modalNavigationStore.CurrentViewModelChanged += OnModalCurrentViewModelChanged;

            _statusMessageStore.CurrentStatusMessageChanged += OnCurrentStatusMessageChanged;

            CloseModalCommand = new NavigationCommand(closeModalNavigationServices);

            CloseSettingsCommand = new NavigationCommand(CloseAdditioNavigationService);

            CloseAppCommand = new AsyncLambdaCommand(OnCloseAppExecuted, ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),CanCloseAppExecute);
        }




        public ICommand CloseAppCommand { get; }

        private bool CanCloseAppExecute(object p) => true;

        private async Task OnCloseAppExecuted(object p)
        {
            _closeAppServices?.Close();
        }

        public ICommand CloseModalCommand { get; }

        public ICommand CloseSettingsCommand { get; }

        private readonly MainWindowNavigationStore _mainWindowNavigationStore;

        private readonly AdditionalNavigationStore _additionalNavigationStore;

        private readonly ModalNavigationStore _modalNavigationStore;

        private  readonly StatusMessageStore _statusMessageStore;

        private readonly ICloseAppServices _closeAppServices;
        private readonly IConfiguration _configuration;


        public BaseVmd? CurrentViewModel => _mainWindowNavigationStore.MainWindowCurrentViewModel;

        public BaseVmd? ModalCurrentViewModel => _modalNavigationStore.ModalMainWindowCurrentViewModel;

        public BaseVmd? AdditionalCurrentViewModel => _additionalNavigationStore.AdditionalMainWindowCurrentViewModel;

        public StatusMessage CurrentStatusMessage => _statusMessageStore.CurrentStatusMessage;



        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }


        private void OnModalCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(ModalCurrentViewModel));

            OnPropertyChanged(nameof(ModalIsOpen));

        }

        private void OnAdditionalCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(AdditionalCurrentViewModel));

            OnPropertyChanged(nameof(AdditionalIsOpen));
        }

        private void OnCurrentStatusMessageChanged()
        {
            OnPropertyChanged(nameof(CurrentStatusMessage));

            OnPropertyChanged(nameof(StatusMessageIsOpen));
        }

        public bool AdditionalIsOpen => _additionalNavigationStore.IsOpen;

        public bool ModalIsOpen => _modalNavigationStore.IsOpen;

        public bool StatusMessageIsOpen => _statusMessageStore.IsOpen;





    }
}
