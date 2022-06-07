using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services
{
    internal class ModalNavigateServices<TViewModel> : INavigationService where TViewModel : BaseVMD
    {
        private readonly AdditionalNavigationStore _AdditionalNavigationStore;

        private readonly Func<TViewModel> _CreateViewModel;

        public ModalNavigateServices(AdditionalNavigationStore additionalNavigationStore, Func<TViewModel> createViewModel)
        {
            _AdditionalNavigationStore = additionalNavigationStore;

            _CreateViewModel = createViewModel;
        }

        public void Navigate()
        {
            _AdditionalNavigationStore.AdditionalMainWindowCurrentViewModel = _CreateViewModel();
        }
    }
}
