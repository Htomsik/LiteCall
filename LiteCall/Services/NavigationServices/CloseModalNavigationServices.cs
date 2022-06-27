using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.NavigationServices;

internal sealed class CloseModalNavigationServices : INavigationService
{
    private readonly ModalNavigationStore _modalNavigationStore;


    public CloseModalNavigationServices(ModalNavigationStore modalNavigationStore)
    {
        _modalNavigationStore = modalNavigationStore;
    }

    public void Navigate()
    {
        _modalNavigationStore.Close();
    }
}