using System.Collections.ObjectModel;
using Core.VMD.Base;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Saved;

public class AppSavedServers : ReactiveObject
{
    [JsonProperty]
    [Reactive]
    public ObservableCollection<ServerAccount>? ServersAccounts { get; set; }
    [JsonProperty]
    public DateTime? LastUpdated { get; set; }
}