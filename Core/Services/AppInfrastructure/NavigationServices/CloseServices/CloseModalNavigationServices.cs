using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices.CloseServices;

public sealed class CloseModalNavigationServices : BaseLazyStoreCloseService<BaseVmd>
{
    public CloseModalNavigationServices(ModalVmdNavigationStore modalVmdNavigationStore) :
        base(modalVmdNavigationStore){}


}