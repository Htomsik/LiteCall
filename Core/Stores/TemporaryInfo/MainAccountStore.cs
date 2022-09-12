using AppInfrastructure.Stores.DefaultStore;
using Core.Models.Users;

namespace Core.Stores.TemporaryInfo;

/// <summary>
///     Store with account of main server
/// </summary>
public class MainAccountStore:BaseLazyCustomDefaultStore<Account>
{
    public bool IsDefaultAccount => CurrentValue.Equals(DefaultValue);
    
    public void Logout() => CurrentValue = null;

    #region Constructors
    public MainAccountStore() : base(null,new Account { Login = "LC_User" }) {}
    
    #endregion
   
}