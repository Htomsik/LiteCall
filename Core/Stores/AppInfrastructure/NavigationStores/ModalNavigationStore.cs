﻿using Core.Stores.AppInfrastructure.NavigationStores.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

/// <summary>
///     Store for modal vmds
/// </summary>
public sealed class ModalNavigationStore : BaseVmdNavigationStore
{
    public bool IsOpen => CurrentValue != null;
    
    public void Close() => CurrentValue = default;
}