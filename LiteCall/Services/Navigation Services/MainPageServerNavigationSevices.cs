using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices
{
    internal class MainPageServerNavigationSevices<TViewModel> : INavigationService where TViewModel : BaseVMD
    {
        private readonly MainPageServerNavigationStore _MainPageServerNavigationStore;

        private readonly Func<TViewModel> _CreateViewModel;

        public MainPageServerNavigationSevices(MainPageServerNavigationStore MainPageServerNavigationStore, Func<TViewModel> createViewModel)
        {
            _MainPageServerNavigationStore = MainPageServerNavigationStore;

            _CreateViewModel = createViewModel;
        }

        public void Navigate()
        {
            _MainPageServerNavigationStore.MainPageServerCurrentViewModel = _CreateViewModel();
        }
    }
}
