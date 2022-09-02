using AppInfrastructure.Stores.DefaultStore;
using Core.Models.AppInfrastructure;

namespace Core.Stores.AppInfrastructure;

public sealed class AppSettingsStore: BaseLazyStore<AppSettings>
{
    public AppSettingsStore()
    {
        CurrentValue = new();
        CurrentValue!.CurrentSettingsChanged += OnCurrentValueChanged;
    }
}