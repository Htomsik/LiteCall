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
        private readonly ModalNavigationStore _ModalNavigationStore;

        private readonly Func<TViewModel> _CreateViewModel;

        public ModalNavigateServices(ModalNavigationStore ModalNavigationStore, Func<TViewModel> createViewModel)
        {
            _ModalNavigationStore = ModalNavigationStore;

            _CreateViewModel = createViewModel;
        }

        public void Navigate()
        {
            _ModalNavigationStore.ModalMainWindowCurrentViewModel = _CreateViewModel();
        }
    }
}
