using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class NavigationSc<TViewModel> : INavigationSc where TViewModel : BaseVmd

{
    private readonly Func<TViewModel> _createViewModel;
    
    private readonly MainWindowVmdNavigationStore _mainWindowVmdNavigationStore;

    public NavigationSc(MainWindowVmdNavigationStore mainWindowVmdNavigationStore, Func<TViewModel> createViewModel)
    {
        _mainWindowVmdNavigationStore = mainWindowVmdNavigationStore;

        _createViewModel = createViewModel;
    }


    public void Navigate()
    {
        _mainWindowVmdNavigationStore.CurrentValue = _createViewModel();
    }
}