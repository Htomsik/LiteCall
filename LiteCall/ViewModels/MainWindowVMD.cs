using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels
{   
    internal class MainWindowVMD:BaseVMD
    {
        public Appsettings Appsettings { get; }

        public MainWindowVMD(MainWindowNavigationStore mainWindowNavigationStore, 
            AdditionalNavigationStore additionalNavigationStore,
            ModalNavigationStore modalNavigationStore,
            StatusMessageStore statusMessageStore,
            INavigationService closeModalNavigationServices,
            INavigationService CloseAdditioNavigationService,
            IStatusServices statusServices,
            ICloseAppSevices closeAppSevices,Appsettings appsettings)
        {
            Appsettings = appsettings;

            _mainWindowNavigationStore = mainWindowNavigationStore;

            _additionalNavigationStore = additionalNavigationStore;

            _modalNavigationStore = modalNavigationStore;

            _statusMessageStore = statusMessageStore;

            _closeAppSevices = closeAppSevices;

            _mainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _additionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;

            _modalNavigationStore.CurrentViewModelChanged += OnModalCurrentViewModelChanged;

            _statusMessageStore.CurentStatusMessageChanged += OnCurentStatusMessageChanged;

            CloseModalCommand = new NavigationCommand(closeModalNavigationServices);

            CloseSettingsCommand = new NavigationCommand(CloseAdditioNavigationService);

            CloseAppCommand = new AsyncLamdaCommand(OnCloseAppExecuted, ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),CanCloseAppExecute);
        }




        public ICommand CloseAppCommand { get; }

        private bool CanCloseAppExecute(object p) => true;

        private async Task OnCloseAppExecuted(object p)
        {
            _closeAppSevices?.Close();
        }

        public ICommand CloseModalCommand { get; }

        public ICommand CloseSettingsCommand { get; }

        private readonly MainWindowNavigationStore _mainWindowNavigationStore;

        private readonly AdditionalNavigationStore _additionalNavigationStore;

        private readonly ModalNavigationStore _modalNavigationStore;

        private  readonly StatusMessageStore _statusMessageStore;

        private readonly ICloseAppSevices _closeAppSevices;


        public BaseVMD CurrentViewModel => _mainWindowNavigationStore.MainWindowCurrentViewModel;

        public BaseVMD ModalCurrentViewModel => _modalNavigationStore.ModalMainWindowCurrentViewModel;

        public BaseVMD AdditionalCurrentViewModel => _additionalNavigationStore.AdditionalMainWindowCurrentViewModel;

        public StatusMessage CurrentStatusMessage => _statusMessageStore.CurentStatusMessage;



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

        private void OnCurentStatusMessageChanged()
        {
            OnPropertyChanged(nameof(CurrentStatusMessage));

            OnPropertyChanged(nameof(StatusMessageIsOpen));
        }

        public bool AdditionalIsOpen => _additionalNavigationStore.IsOpen;

        public bool ModalIsOpen => _modalNavigationStore.IsOpen;

        public bool StatusMessageIsOpen => _statusMessageStore.IsOpen;





    }
}
