using AppInfrastructure.Services.NavigationServices.Navigation;
using AppInfrastructure.Services.StoreServices;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class ModalNavigateServices :  BaseLazyStoreNavigationService<BaseVmd>
{
    public ModalNavigateServices(ModalVmdNavigationStore modalVmdNavigationStore, Func<BaseVmd> createViewModel) : base(modalVmdNavigationStore, createViewModel){}
}