using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class MainPageServerNavigationScs<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly MainPageServerNavigationStore _mainPageServerNavigationStore;

    public MainPageServerNavigationScs(MainPageServerNavigationStore MainPageServerNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _mainPageServerNavigationStore = MainPageServerNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _mainPageServerNavigationStore.CurrentValue = _createViewModel();
    }
}