using AppInfrastructure.Stores.DefaultStore;
using Core.Models.Users;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AppInfrastructure.FileServices.Base;

/// <summary>
///     Abstraction realizing of file service. All data placed in "LiteCall/Data/" 
/// </summary>
/// <typeparam name="TValue">Some generic value</typeparam>
public abstract class DataFileService<TValue> : BaseStoreFileService<TValue>
{
    #region Fields

    private MainAccountStore _mainAccountStore;

    #endregion
    
    #region Constructors

    public DataFileService(MainAccountStore mainAccountStore,IStore<TValue> store) : base(store)
    {

        #region Properties and Fields Initializing

        _mainAccountStore = mainAccountStore;

        #endregion
        
        #region Initializing

        BeforeSaving();
        
        #endregion
    }

    #endregion


    protected override void BeforeSaving()
    {
        DirectoryPath = _mainAccountStore.IsDefaultAccount
            ? "Data/NonAuthorized"
            : $"Data/{_mainAccountStore.CurrentValue.GuidId}";
    }

    protected override string DirectoryPath { get; set; }

    protected abstract override string FileName { get; } 
}