using Core.Models.Users;
using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Saved;

public sealed class CurrentAccountSavedServers : AppSavedServers
{
    [Reactive]
    [JsonProperty]
    public Account? MainServerAccount { get; set; }
}