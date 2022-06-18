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
        public MainWindowVMD(MainWindowNavigationStore mainWindowNavigationStore, AdditionalNavigationStore additionalNavigationStore,ModalNavigationStore modalNavigationStore,StatusMessageStore statusMessageStore, INavigationService closeModalNavigationServices, INavigationService CloseAdditioNavigationService)
        {
            _MainWindowNavigationStore = mainWindowNavigationStore;

            _AdditionalNavigationStore = additionalNavigationStore;

            _ModalNavigationStore = modalNavigationStore;

            _StatusMessageStore = statusMessageStore;

            _MainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _AdditionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;

            _ModalNavigationStore.CurrentViewModelChanged += OnModalCurrentViewModelChanged;

            _StatusMessageStore.CurentStatusMessageChanged += OnCurentStatusMessageChanged;

            CloseModalCommand = new NavigationCommand(closeModalNavigationServices);

            CloseSettingsCommand = new NavigationCommand(CloseAdditioNavigationService);
        }


        public ICommand CloseModalCommand { get; }

        public ICommand CloseSettingsCommand { get; }

        private readonly MainWindowNavigationStore _MainWindowNavigationStore;

        private readonly AdditionalNavigationStore _AdditionalNavigationStore;

        private readonly ModalNavigationStore _ModalNavigationStore;

        private  readonly StatusMessageStore _StatusMessageStore;


        public BaseVMD CurrentViewModel => _MainWindowNavigationStore.MainWindowCurrentViewModel;

        public BaseVMD ModalCurrentViewModel => _ModalNavigationStore.ModalMainWindowCurrentViewModel;

        public BaseVMD AdditionalCurrentViewModel => _AdditionalNavigationStore.AdditionalMainWindowCurrentViewModel;

        public StatusMessage CurrentStatusMessage => _StatusMessageStore.CurentStatusMessage;



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

        public bool AdditionalIsOpen => _AdditionalNavigationStore.IsOpen;

        public bool ModalIsOpen => _ModalNavigationStore.IsOpen;

        public bool StatusMessageIsOpen => _StatusMessageStore.IsOpen;





    }
}
