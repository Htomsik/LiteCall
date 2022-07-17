using System.Collections.ObjectModel;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.Stores.TemporaryInfo;

public sealed class SavedServersStore : BaseVmd
{
    private AppSavedServers? _savedServerAccounts =
        new() { ServersAccounts = new ObservableCollection<ServerAccount>() };

    public AppSavedServers? SavedServerAccounts
    {
        get => _savedServerAccounts;
        set
        {
            this.RaiseAndSetIfChanged(ref _savedServerAccounts, value);
            OnCurrentSeverAccountChanged();
        }
    }


    public event Action? ServersAccountsChange;

    private void OnCurrentSeverAccountChanged()
    {
        ServersAccountsChange?.Invoke();
    }


    public void Add(ServerAccount newServerAccount)
    {
        ServerAccount? findAccount = null;

        try
        {
            findAccount =
                SavedServerAccounts?.ServersAccounts?.First(x =>
                    x.SavedServer!.ApiIp == newServerAccount.SavedServer!.ApiIp);
        }
        catch
        {
            // ignored
        }


        if (findAccount != null) throw new Exception("Server already added");
        
        if (SavedServerAccounts!.ServersAccounts is null)
            SavedServerAccounts.ServersAccounts = new ObservableCollection<ServerAccount>();

        SavedServerAccounts.ServersAccounts.Add(newServerAccount);
        
        OnCurrentSeverAccountChanged();
    }

    public void Remove(ServerAccount? deletedServer)
    {
        try
        {
            var findAccount = SavedServerAccounts?.ServersAccounts?.First(x =>
                x.SavedServer!.ApiIp == deletedServer!.SavedServer!.ApiIp)!;
            
            SavedServerAccounts?.ServersAccounts?.Remove(findAccount);
            OnCurrentSeverAccountChanged();
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
                SavedServerAccounts!.ServersAccounts?.First(x => x.SavedServer!.ApiIp == replacedServer!.ApiIp);

            SavedServerAccounts.ServersAccounts?.Remove(findAccount!);

            findAccount!.Account = newAccount;

            SavedServerAccounts.ServersAccounts?.Add(findAccount);
        }
        catch
        {
            findAccount = new ServerAccount
            {
                Account = newAccount,
                SavedServer = replacedServer
            };

            SavedServerAccounts?.ServersAccounts?.Add(findAccount);
        }


        OnCurrentSeverAccountChanged();
    }
}