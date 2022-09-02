using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class MainPageServerNavigationService : BaseLazyStoreNavigationService<BaseVmd>
{
    public MainPageServerNavigationService(CurrentServerVmdNavigationStore currentServerVmdNavigationStore,
        Func<BaseVmd> createViewModel) : base(currentServerVmdNavigationStore, createViewModel){}



}