using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class MainPageVmdsNavigationService : BaseLazyStoreNavigationService<BaseVmd>

{
    public MainPageVmdsNavigationService(MainWindowVmdNavigationStore mainWindowVmdNavigationStore, Func<BaseVmd> createViewModel) : base(mainWindowVmdNavigationStore, createViewModel){}
        
}