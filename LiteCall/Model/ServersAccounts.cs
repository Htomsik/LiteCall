using System;
using System.Collections.ObjectModel;
using LiteCall.ViewModels.Base;
using Newtonsoft.Json;

namespace LiteCall.Model;

internal sealed class ServerAccount
{
    public Server? SavedServer { get; set; }

    public Account? Account { get; set; }
}

internal sealed class SavedServers : AppSavedServers
{
    public Account? MainServerAccount { get; set; } = null;
}

internal class AppSavedServers : BaseVmd
{
    [JsonIgnore] private ObservableCollection<ServerAccount>? _serversAccounts = new();

    public ObservableCollection<ServerAccount>? ServersAccounts
    {
        get => _serversAccounts;
        set => Set(ref _serversAccounts, value);
    }


    public DateTime? LastUpdated { get; set; }
}