﻿using Core.Stores.AppInfrastructure.NavigationStores.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

/// <summary>
///     Additional vmds store (Settings/modal and other)
/// </summary>
public sealed class AdditionalVmdsNavigationStore : BaseVmdNavigationStore
{
    
    public void Close() => CurrentValue = default;
    
}