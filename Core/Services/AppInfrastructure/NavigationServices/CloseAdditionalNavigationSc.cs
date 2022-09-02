using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class CloseAdditionalNavigationSc : INavigationSc
{
    private readonly AdditionalVmdsNavigationStore _vmdsNavigationStore;

    public CloseAdditionalNavigationSc(AdditionalVmdsNavigationStore vmdsNavigationStore)
    {
        _vmdsNavigationStore = vmdsNavigationStore;
    }

    public void Navigate()
    {
        _vmdsNavigationStore.Close();
    }
}