using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class SettingsAccountManagementVmdFullNavigationServices : BaseLazyStoreFullNavigationService<BaseVmd>
{
    public SettingsAccountManagementVmdFullNavigationServices(SettingsAccountManagementVmdNavigationStore settingsAccountManagementVmdNavigationStore,
        Func<BaseVmd> createViewModel) : base(settingsAccountManagementVmdNavigationStore,createViewModel){}
}