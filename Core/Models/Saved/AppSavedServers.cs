using System.Collections.ObjectModel;
using Core.VMD.Base;
using Newtonsoft.Json;
using ReactiveUI;

namespace Core.Models.Saved;

public class AppSavedServers : BaseVmd
{
    [JsonIgnore] private ObservableCollection<ServerAccount>? _serversAccounts = new();

    public ObservableCollection<ServerAccount>? ServersAccounts
    {
        get => _serversAccounts;
        set => this.RaiseAndSetIfChanged(ref _serversAccounts, value);
    }
    
    public DateTime? LastUpdated { get; set; }
}