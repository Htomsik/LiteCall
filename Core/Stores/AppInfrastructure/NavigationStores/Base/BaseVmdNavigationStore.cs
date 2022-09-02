using AppInfrastructure.Stores.DefaultStore;
using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores.Base;

public class BaseVmdNavigationStore : BaseLazyStore<BaseVmd>
{
    public override BaseVmd? CurrentValue
    {
        get => (BaseVmd?)_currentValue.Value;
        set
        {
            (((BaseVmd?)_currentValue.Value))?.Dispose();
            _currentValue = new Lazy<object>( (() => value));
            OnCurrentValueChanged();
        }
    }
}