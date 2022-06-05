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
        public MainWindowVMD(MainWindowNavigationStore mainWindowNavigationStore, AdditionalNavigationStore additionalNavigationStore)
        {
            _mainWindowNavigationStore = mainWindowNavigationStore;

            _AdditionalNavigationStore = additionalNavigationStore;

            _mainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _AdditionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;
        }

       
        private readonly MainWindowNavigationStore _mainWindowNavigationStore;

        private readonly AdditionalNavigationStore _AdditionalNavigationStore;



        public BaseVMD CurrentViewModel => _mainWindowNavigationStore.MainWindowCurrentViewModel;

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
