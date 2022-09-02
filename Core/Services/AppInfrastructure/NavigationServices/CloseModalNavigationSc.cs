using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class CloseModalNavigationSc : INavigationSc
{
    private readonly ModalVmdNavigationStore _modalVmdNavigationStore;


    public CloseModalNavigationSc(ModalVmdNavigationStore modalVmdNavigationStore)
    {
        _modalVmdNavigationStore = modalVmdNavigationStore;
    }

    public void Navigate()
    {
        _modalVmdNavigationStore.CurrentValue = default;
    }
}