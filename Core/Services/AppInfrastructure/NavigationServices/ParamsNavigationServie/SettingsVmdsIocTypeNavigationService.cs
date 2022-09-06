using Core.Services.AppInfrastructure.NavigationServices.ParamsNavigationServie.Base;
using Core.Services.Retranslators.Base;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.AdditionalVmds.SettingsVmds;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices.ParamsNavigationServie;

/// <summary>
///         Navigation service fore settings vmds
/// </summary>
public class SettingsVmdsIocTypeNavigationService : BaseIocTypeNavigationService
{
    public SettingsVmdsIocTypeNavigationService(SettingsVmdsNavigationStore store, IRetranslor<Type, BaseVmd> iocRetranslator) : base(store, iocRetranslator)
    {
        Navigate(typeof(AboutProgramSettingsVmd));
    }
}