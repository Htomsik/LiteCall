using AppInfrastructure.Stores.DefaultStore;
using Core.Models.AppInfrastructure;

namespace Core.Stores.AppInfrastructure;


public class AppExecutionStateStore : BaseLazyStore<AppExecutionState>
{
    public bool IsOpen => !string.IsNullOrEmpty(CurrentValue?.Message);
    
}