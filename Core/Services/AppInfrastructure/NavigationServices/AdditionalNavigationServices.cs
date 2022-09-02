using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class AdditionalNavigationServices<TViewModel> : INavigationServices where TViewModel : BaseVmd
{
    private readonly AdditionalVmdsNavigationStore _additionalVmdsNavigationStore;

    private readonly Func<TViewModel> _createViewModel;

    public AdditionalNavigationServices(AdditionalVmdsNavigationStore additionalVmdsNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _additionalVmdsNavigationStore = additionalVmdsNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _additionalVmdsNavigationStore.CurrentValue = _createViewModel();
    }
}