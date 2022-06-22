using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class SavedServersStore : BaseVmd
{
    private AppSavedServers? _savedServerAccounts =
        new() { ServersAccounts = new ObservableCollection<ServerAccount>() };

    public AppSavedServers? SavedServerAccounts
    {
        get => _savedServerAccounts;
        set
        {
            Set(ref _savedServerAccounts, value);
            OnCurrentSeverAccountChanged();
        }
    }


    public event Action? ServersAccountsChange;

    private void OnCurrentSeverAccountChanged()
    {
        ServersAccountsChange?.Invoke();
    }


    public bool Add(ServerAccount newServerAccount)
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


        if (findAccount == null)
        {
            if (SavedServerAccounts!.ServersAccounts is null)
                SavedServerAccounts.ServersAccounts = new ObservableCollection<ServerAccount>();

            SavedServerAccounts.ServersAccounts.Add(newServerAccount);
            OnCurrentSeverAccountChanged();
            return true;
        }

        return false;
    }

    public void Remove(ServerAccount? deletedServer)
    {
        ServerAccount findAccount = null!;

        try
        {
            findAccount =
                SavedServerAccounts?.ServersAccounts?.First(x =>
                    x.SavedServer!.ApiIp == deletedServer.SavedServer!.ApiIp)!;
        }
        catch 
        {
            // ignored
        }


        SavedServerAccounts?.ServersAccounts?.Remove(findAccount);
        OnCurrentSeverAccountChanged();
    }

    public void Replace(Server? ReplacedServer, Account? newAccount)
    {
        ServerAccount? findAccount = null;


        try
        {
            findAccount = SavedServerAccounts!.ServersAccounts?.First(x => x.SavedServer.ApiIp == ReplacedServer.ApiIp);

            SavedServerAccounts.ServersAccounts?.Remove(findAccount!);

            findAccount!.Account = newAccount;

            SavedServerAccounts.ServersAccounts?.Add(findAccount);
        }
        catch 
        {
            findAccount = new ServerAccount
            {
                Account = newAccount,
                SavedServer = ReplacedServer
            };

            SavedServerAccounts?.ServersAccounts?.Add(findAccount);
        }


        OnCurrentSeverAccountChanged();
    }
}