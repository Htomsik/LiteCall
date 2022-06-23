﻿using System;
using LiteCall.Services.Interfaces;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices;

internal class AdditionalNavigationServices<TViewModel> : INavigationService where TViewModel : BaseVmd
{
    private readonly AdditionalNavigationStore _additionalNavigationStore;

    private readonly Func<TViewModel> _createViewModel;

    public AdditionalNavigationServices(AdditionalNavigationStore additionalNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _additionalNavigationStore = additionalNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _additionalNavigationStore.AdditionalMainWindowCurrentViewModel = _createViewModel();
    }
}