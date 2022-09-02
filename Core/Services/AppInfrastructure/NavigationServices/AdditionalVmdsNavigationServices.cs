using AppInfrastructure.Services.NavigationServices.Navigation;
using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class AdditionalVmdsNavigationServices : BaseLazyStoreNavigationService<BaseVmd>
{
    
    public AdditionalVmdsNavigationServices(AdditionalVmdsNavigationStore additionalVmdsNavigationStore,
        Func<BaseVmd> createViewModel) : base(additionalVmdsNavigationStore,createViewModel)
    {
        
    }
    
}