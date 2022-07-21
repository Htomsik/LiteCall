using Core.Models.Users;
using Newtonsoft.Json;

namespace Core.Models.Saved;

public sealed class CurrentAccountSavedServers : AppSavedServers
{
    [JsonProperty]
    public Account? MainServerAccount { get; set; }
}