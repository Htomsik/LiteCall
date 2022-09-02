﻿using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class MainPageServerNavigationScs<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly ServerVmdNavigationStore _serverVmdNavigationStore;

    public MainPageServerNavigationScs(ServerVmdNavigationStore serverVmdNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _serverVmdNavigationStore = serverVmdNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _serverVmdNavigationStore.CurrentValue = _createViewModel();
    }
}