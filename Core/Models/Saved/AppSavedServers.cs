using System.Collections.ObjectModel;
using Core.VMD.Base;
using Newtonsoft.Json;

namespace Core.Models.Saved;

public class AppSavedServers : BaseVmd
{
    [JsonIgnore] private ObservableCollection<ServerAccount>? _serversAccounts = new();

    public ObservableCollection<ServerAccount>? ServersAccounts
    {
        get => _serversAccounts;
        set => Set(ref _serversAccounts, value);
    }
    
    public DateTime? LastUpdated { get; set; }
}