using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Users;

public class Account:RegistrationUser
{
    
   [Reactive]
   [JsonIgnore]
    public string? CurrentServerLogin { get; set; }
  
    [Reactive]
    [JsonIgnore]
    public string? Role { get; set; }

    [Reactive]
    [JsonIgnore]
    public string? Token { get; set; }

    [Reactive]
    [JsonProperty]
    public bool IsAuthorized { get; set; }
}