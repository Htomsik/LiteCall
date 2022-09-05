using AppInfrastructure.Stores.DefaultStore;
using Core.Models.AppInfrastructure;
using Core.Stores.AppInfrastructure;
using Core.VMD.Base;

namespace Core.VMD.AdditionalVmds.SettingsVmds.Base;

/// <summary>
///     Base abstract realize for Settings vmds
/// </summary>
public abstract class BaseSettingsVmd : BaseVmd
{
    #region Constructors

    public BaseSettingsVmd(AppSettingsStore appSettingsStore)
    {
        #region Properties and Fields Initializing

        _lazyAppSettingsStore = new Lazy<IStore<AppSettings>>(()=> appSettingsStore);

        #endregion
    }

    #endregion

    #region Stores

    protected Lazy<IStore<AppSettings>> _lazyAppSettingsStore;

    #endregion
}