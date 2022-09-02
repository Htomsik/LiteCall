using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class NavigationSc<TViewModel> : INavigationSc where TViewModel : BaseVmd

{
    private readonly Func<TViewModel> _createViewModel;
    
    private readonly MainWindowNavigationStore _mainWindowNavigationStore;

    public NavigationSc(MainWindowNavigationStore mainWindowNavigationStore, Func<TViewModel> createViewModel)
    {
        _mainWindowNavigationStore = mainWindowNavigationStore;

        _createViewModel = createViewModel;
    }


    public void Navigate()
    {
        _mainWindowNavigationStore.CurrentValue = _createViewModel();
    }
}