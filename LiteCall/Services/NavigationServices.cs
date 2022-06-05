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
        private readonly NavigationStore _NavigationStore;

        private readonly Func<TViewModel> _CreateViewModel;

        public NavigationServices(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _NavigationStore = navigationStore;

            _CreateViewModel = createViewModel;
        }

        public void Navigate()
        {
            _NavigationStore.MainWindowCurrentViewModel = _CreateViewModel();
        }
    }
}
