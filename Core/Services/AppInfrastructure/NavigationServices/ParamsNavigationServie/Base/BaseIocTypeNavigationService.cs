using AppInfrastructure.Services.StoreServices.Parametrize;
using AppInfrastructure.Stores.DefaultStore;
using Core.Services.Retranslations.Base;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices.ParamsNavigationServie.Base;

/// <summary>
///     Base realizing for param navigation
/// </summary>
public class BaseIocTypeNavigationService : BaseLazyParamNavigationService<Type,BaseVmd>
{
    public BaseIocTypeNavigationService(IStore<BaseVmd> store, IRetranslor<Type,BaseVmd> iocRetranslator) : base(store, (type => iocRetranslator.Retranslate(type) ))
    {
    }
}