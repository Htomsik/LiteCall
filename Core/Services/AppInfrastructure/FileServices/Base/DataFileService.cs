using AppInfrastructure.Stores.DefaultStore;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AppInfrastructure.FileServices.Base;

public abstract class DataFileService<TValue> : BaseStoreFileService<TValue>
{
    #region Constructors

    public DataFileService(MainAccountStore mainAccount,IStore<TValue> store) : base(store)
    {
        #region Subscription

        store.CurrentValueChangedNotifier += () =>
        {
            DirectoryPath = mainAccount.IsDefaultAccount
                ? "Data/NonAuthorized"
                : $"Data/{mainAccount.CurrentValue.GuidId}";
        };

        #endregion

        #region Fileds Initializing
        
        DirectoryPath = mainAccount.IsDefaultAccount
            ? "Data/NonAuthorized"
            : $"Data/{mainAccount.CurrentValue.GuidId}";
        
        #endregion
        
    }

    #endregion
    
    protected override string DirectoryPath { get; set; }

    protected abstract override string FileName { get; } 
}