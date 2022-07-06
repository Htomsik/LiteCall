using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class ModalNavigateSc<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly ModalNavigationStore _modalNavigationStore;

    public ModalNavigateSc(ModalNavigationStore modalNavigationStore, Func<TViewModel> createViewModel)
    {
        _modalNavigationStore = modalNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _modalNavigationStore.ModalMainWindowCurrentViewModel = _createViewModel();
    }
}