using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices.CloseServices;

public sealed class CloseAdditionalNavigationServices : BaseLazyStoreCloseService<BaseVmd>
{
    public CloseAdditionalNavigationServices(AdditionalVmdsNavigationStore vmdsNavigationStore) : base(vmdsNavigationStore){}
}