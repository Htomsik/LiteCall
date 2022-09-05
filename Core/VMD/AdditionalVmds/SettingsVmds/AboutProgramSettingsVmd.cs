using Core.Stores.AppInfrastructure;
using Core.VMD.AdditionalVmds.SettingsVmds.Base;

namespace Core.VMD.AdditionalVmds.SettingsVmds;

public class AboutProgramSettingsVmd : BaseSettingsVmd
{
    public AboutProgramSettingsVmd(AppSettingsStore appSettingsStore) : base(appSettingsStore)
    {
    }
}