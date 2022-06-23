using System;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices;

internal sealed class ModalNavigateServices<TViewModel> : INavigationService where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly ModalNavigationStore _modalNavigationStore;

    public ModalNavigateServices(ModalNavigationStore modalNavigationStore, Func<TViewModel> createViewModel)
    {
        _modalNavigationStore = modalNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _modalNavigationStore.ModalMainWindowCurrentViewModel = _createViewModel();
    }
}