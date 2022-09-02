using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Saved;

public class AppSavedServers : ReactiveObject
{
    [Reactive]
    [JsonProperty]
    public ObservableCollection<ServerAccount>? ServersAccounts { get; set; }
    [JsonProperty]
    public DateTime? LastUpdated { get; set; }
}