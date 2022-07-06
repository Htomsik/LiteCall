using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class CloseModalNavigationSc : INavigationSc
{
    private readonly ModalNavigationStore _modalNavigationStore;


    public CloseModalNavigationSc(ModalNavigationStore modalNavigationStore)
    {
        _modalNavigationStore = modalNavigationStore;
    }

    public void Navigate()
    {
        _modalNavigationStore.Close();
    }
}