using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class CloseAdditionalNavigationSc : INavigationSc
{
    private readonly AdditionalNavigationStore _navigationStore;

    public CloseAdditionalNavigationSc(AdditionalNavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public void Navigate()
    {
        _navigationStore.Close();
    }
}