using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels
{   
    internal class MainWindowVMD:BaseVMD
    {
        public MainWindowVMD(MainWindowNavigationStore mainWindowNavigationStore, AdditionalNavigationStore additionalNavigationStore,ModalNavigationStore modalNavigationStore, INavigationService closeModalNavigationServices)
        {
            _MainWindowNavigationStore = mainWindowNavigationStore;

            _AdditionalNavigationStore = additionalNavigationStore;

            _ModalNavigationStore = modalNavigationStore;

            _MainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _AdditionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;

            _ModalNavigationStore.CurrentViewModelChanged += OnModalCurrentViewModelChanged;

            CloseModalCommand = new NavigationCommand(closeModalNavigationServices);
        }


        public ICommand CloseModalCommand { get; }

        private readonly MainWindowNavigationStore _MainWindowNavigationStore;

        private readonly AdditionalNavigationStore _AdditionalNavigationStore;

        private readonly ModalNavigationStore _ModalNavigationStore;


        public BaseVMD CurrentViewModel => _MainWindowNavigationStore.MainWindowCurrentViewModel;

        public BaseVMD ModalCurrentViewModel => _ModalNavigationStore.ModalMainWindowCurrentViewModel;

        public BaseVMD AdditionalCurrentViewModel => _AdditionalNavigationStore.AdditionalMainWindowCurrentViewModel ;



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

        public bool AdditionalIsOpen => _AdditionalNavigationStore.IsOpen;

        public bool ModalIsOpen => _ModalNavigationStore.IsOpen;



    }
}
