using Core.Stores.AppInfrastructure.NavigationStores.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

/// <summary>
///     Store vmds for settings
/// </summary>
public sealed class SettingsAccNavigationStore : BaseVmdNavigationStore
{
    public void Close() => CurrentValue = null;
    
}