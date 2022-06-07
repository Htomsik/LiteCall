using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services
{
    internal class NavigationServices<TViewModel> : INavigationService where TViewModel : BaseVMD

    {
        private readonly MainWindowNavigationStore _mainWindowNavigationStore;

        private readonly Func<TViewModel> _CreateViewModel;

        public NavigationServices(MainWindowNavigationStore mainWindowNavigationStore, Func<TViewModel> createViewModel)
        {
            _mainWindowNavigationStore = mainWindowNavigationStore;

            _CreateViewModel = createViewModel;
        }

        public void Navigate()
        {
            _mainWindowNavigationStore.MainWindowCurrentViewModel = _CreateViewModel();
        }
    }
}
