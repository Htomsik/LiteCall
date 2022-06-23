using System;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices;

internal sealed class NavigationServices<TViewModel> : INavigationService where TViewModel : BaseVmd

{
    private readonly Func<TViewModel> _createViewModel;
    private readonly MainWindowNavigationStore _mainWindowNavigationStore;

    public NavigationServices(MainWindowNavigationStore mainWindowNavigationStore, Func<TViewModel> createViewModel)
    {
        _mainWindowNavigationStore = mainWindowNavigationStore;

        _createViewModel = createViewModel;
    }


    public void Navigate()
    {
        _mainWindowNavigationStore.MainWindowCurrentViewModel = _createViewModel();
    }
}