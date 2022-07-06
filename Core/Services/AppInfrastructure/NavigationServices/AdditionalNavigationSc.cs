using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class AdditionalNavigationSc<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly AdditionalNavigationStore _additionalNavigationStore;

    private readonly Func<TViewModel> _createViewModel;

    public AdditionalNavigationSc(AdditionalNavigationStore additionalNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _additionalNavigationStore = additionalNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _additionalNavigationStore.AdditionalMainWindowCurrentViewModel = _createViewModel();
    }
}