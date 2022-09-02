using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class ModalNavigateSc<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly ModalVmdNavigationStore _modalVmdNavigationStore;

    public ModalNavigateSc(ModalVmdNavigationStore modalVmdNavigationStore, Func<TViewModel> createViewModel)
    {
        _modalVmdNavigationStore = modalVmdNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _modalVmdNavigationStore.CurrentValue = _createViewModel();
    }
}