using AppInfrastructure.Stores.DefaultStore;
using DynamicData.Binding;
using ReactiveUI;

namespace Core.Stores.Base;

/// <summary>
///     Base store for reactive data
/// </summary>
public class BaseReactiveDateStore<TValue> : BaseLazyStore<TValue> where TValue : IReactiveObject
{
   public override TValue? CurrentValue
   {
       get => (TValue?)_currentValue.Value;
       set
       {
           _currentValue = new Lazy<object?>(()=> value);
            
           CurrentValue?.WhenAnyPropertyChanged()
               .Subscribe(_ => OnCurrentValueChanged());
            
           OnCurrentValueChanged();
       }
      
   }
}