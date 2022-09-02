using AppInfrastructure.Stores.DefaultStore;
using Core.Models.Users;

namespace Core.Stores.TemporaryInfo;

/// <summary>
///     Store wit Account of main server
/// </summary>
public class MainAccountStore:BaseLazyStore<Account>
{
    private readonly Account? _defaultAccount = new Account { Login = "LC_User" };
    public MainAccountStore() =>  CurrentValue = _defaultAccount;
    
    public bool IsDefaultAccount => CurrentValue == _defaultAccount;
    
    public void Logout() => CurrentValue = _defaultAccount;
  
}