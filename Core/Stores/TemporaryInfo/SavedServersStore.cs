using System.Collections.ObjectModel;
using AppInfrastructure.Stores.DefaultStore;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.Stores.TemporaryInfo;

public sealed class SavedServersStore : BaseLazyStore<AppSavedServers>
{
    
    public void Add(ServerAccount newServerAccount)
    {
        ServerAccount? findAccount = null;

        try
        {
            findAccount =
                CurrentValue?.ServersAccounts?.First(x =>
                    x.SavedServer!.ApiIp == newServerAccount.SavedServer!.ApiIp);
        }
        catch (Exception)
        {
            // ignored
        }
        
        if (findAccount != null) throw new Exception("Server already added");
        
        if (CurrentValue?.ServersAccounts is null)
            CurrentValue.ServersAccounts = new ObservableCollection<ServerAccount>();

        CurrentValue?.ServersAccounts.Add(newServerAccount);
        
        OnCurrentValueChanged();
    }

    public void Remove(ServerAccount? deletedServer)
    {
        try
        {
            var findAccount = CurrentValue?.ServersAccounts?.First(x =>
                x.SavedServer!.ApiIp == deletedServer!.SavedServer!.ApiIp)!;
            
            CurrentValue?.ServersAccounts?.Remove(findAccount);
            OnCurrentValueChanged();
        }
        catch
        {
            throw new Exception("Saved server doesn't exist");
        }
        
    }

    public void Replace(Server? replacedServer, Account? newAccount)
    {
        ServerAccount? findAccount = null;
        
        try
        {
            findAccount =
                CurrentValue?.ServersAccounts?.First(x => x.SavedServer!.ApiIp == replacedServer!.ApiIp);

            CurrentValue.ServersAccounts?.Remove(findAccount!);

            findAccount!.Account = newAccount;

            CurrentValue.ServersAccounts?.Add(findAccount);
        }
        catch
        {
            findAccount = new ServerAccount
            {
                Account = newAccount,
                SavedServer = replacedServer
            };

            CurrentValue?.ServersAccounts?.Add(findAccount);
        }


        OnCurrentValueChanged();
    }
}