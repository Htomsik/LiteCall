﻿using AppInfrastructure.Services.NavigationServices.Navigation;
using AppInfrastructure.Services.StoreServices;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class MainPageServerNavigationService : BaseLazyStoreNavigationService<BaseVmd>
{
    public MainPageServerNavigationService(ServerVmdNavigationStore serverVmdNavigationStore,
        Func<BaseVmd> createViewModel) : base(serverVmdNavigationStore, createViewModel){}



}