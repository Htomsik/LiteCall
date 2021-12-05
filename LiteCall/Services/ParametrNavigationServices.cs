using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services
{
    internal class ParametrNavigationServices<TParameter,TViewModel>
        where TViewModel : BaseVMD
    {
        private readonly NavigationStore _NavigationStore;
        private readonly Func<TParameter,TViewModel> _CreateViewModel;

        public ParametrNavigationServices(NavigationStore navigationStore, Func<TParameter, TViewModel> createViewModel)
        {
            _NavigationStore = navigationStore;
            _CreateViewModel = createViewModel;
        }

        public void Navigate(TParameter parameter)
        {
            _NavigationStore.MainWindowCurrentViewModel = _CreateViewModel(parameter);
        }
    }

}
