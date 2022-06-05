using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels
{   
    internal class MainWindowVMD:BaseVMD
    {
        public MainWindowVMD(NavigationStore navigationStore, AdditionalNavigationStore additionalNavigationStore)
        {
            _NavigationStore = navigationStore;

            _AdditionalNavigationStore = additionalNavigationStore;

            _NavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _AdditionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;
        }

       
        private readonly NavigationStore _NavigationStore;

        private readonly AdditionalNavigationStore _AdditionalNavigationStore;



        public BaseVMD CurrentViewModel => _NavigationStore.MainWindowCurrentViewModel;

        public BaseVMD AdditionalCurrentViewModel => _AdditionalNavigationStore.AdditionalMainWindowCurrentViewModel ;



        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }


        private void OnAdditionalCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(AdditionalCurrentViewModel));

            OnPropertyChanged(nameof(IsOpen));
        }

        public bool IsOpen => _AdditionalNavigationStore.IsOpen;



    }
}
