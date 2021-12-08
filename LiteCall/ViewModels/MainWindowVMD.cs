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
        public MainWindowVMD(NavigationStore navigationStore)
        {
            _NavigationStore = navigationStore;

            _NavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private readonly NavigationStore _NavigationStore;

        public BaseVMD CurrentViewModel => _NavigationStore.MainWindowCurrentViewModel;

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
