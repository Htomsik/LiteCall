using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.NavigationServices;

internal sealed class CloseAdditionalNavigationServices : INavigationService
{
    private readonly AdditionalNavigationStore _navigationStore;

    public CloseAdditionalNavigationServices(AdditionalNavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public void Navigate()
    {
        _navigationStore.Close();
    }
}