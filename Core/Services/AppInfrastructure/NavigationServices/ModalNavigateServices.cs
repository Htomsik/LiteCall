using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class ModalNavigateServices<TViewModel> : INavigationServices where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly ModalVmdNavigationStore _modalVmdNavigationStore;

    public ModalNavigateServices(ModalVmdNavigationStore modalVmdNavigationStore, Func<TViewModel> createViewModel)
    {
        _modalVmdNavigationStore = modalVmdNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _modalVmdNavigationStore.CurrentValue = _createViewModel();
    }
}