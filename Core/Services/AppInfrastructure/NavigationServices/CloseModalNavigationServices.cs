using AppInfrastructure.Services.NavigationServices.Close;
using AppInfrastructure.Services.NavigationServices.Navigation;
using AppInfrastructure.Services.StoreServices;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class CloseModalNavigationServices : BaseLazyStoreCloseService<BaseVmd>
{
    public CloseModalNavigationServices(ModalVmdNavigationStore modalVmdNavigationStore) :
        base(modalVmdNavigationStore){}


}