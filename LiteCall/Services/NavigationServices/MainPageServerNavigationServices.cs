using System;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices;

internal class MainPageServerNavigationServices<TViewModel> : INavigationService where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly MainPageServerNavigationStore _mainPageServerNavigationStore;

    public MainPageServerNavigationServices(MainPageServerNavigationStore MainPageServerNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _mainPageServerNavigationStore = MainPageServerNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _mainPageServerNavigationStore.MainPageServerCurrentViewModel = _createViewModel();
    }
}