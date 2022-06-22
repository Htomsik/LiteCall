﻿using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.NavigationStores;

namespace LiteCall.Services.NavigationServices;

internal class CloseAdditionalNavigationServices : INavigationService
{
    private readonly AdditionalNavigationStore _navigationStore;

    public CloseAdditionalNavigationServices(AdditionalNavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public void Navigate()
    {
        _navigationStore.Close();
    }
}