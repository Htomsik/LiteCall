using AppInfrastructure.Services.StoreServices;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class SettingAccNavigationServices : BaseLazyStoreNavigationService<BaseVmd>
{
    public SettingAccNavigationServices(SettingsAccountVmdNavigationStore settingsAccountVmdNavigationStore,
        Func<BaseVmd> createViewModel) : base(settingsAccountVmdNavigationStore,createViewModel){}
}