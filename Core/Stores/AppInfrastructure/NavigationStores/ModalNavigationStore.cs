using Core.Stores.AppInfrastructure.NavigationStores.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

/// <summary>
///     Store for modal vmds
/// </summary>
public sealed class ModalNavigationStore : BaseVmdNavigationStore
{
    public void Close() => CurrentValue = default;
}