using AppInfrastructure.Stores.DefaultStore;
using Core.Models.Users;
using DynamicData.Binding;

namespace Core.Stores.TemporaryInfo;

public sealed class CurrentServerAccountStore : BaseLazyStore<Account>
{
    public override Account? CurrentValue
    {
        get => (Account?)_currentValue.Value;
        set
        {
            _currentValue = new Lazy<object?>(()=> value);
            
            CurrentValue?.WhenAnyPropertyChanged()
                    .Subscribe(_ => OnCurrentValueChanged());
            
            OnCurrentValueChanged();
        }
        
    }
}