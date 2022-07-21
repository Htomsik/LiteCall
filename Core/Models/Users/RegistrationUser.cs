using Newtonsoft.Json;
using ReactiveUI;

namespace Core.Models.Users;

public class RegistrationUser:ReactiveObject
{
    [JsonProperty]
    public string? Login { get; set; }
    
    public string? Password { get; set; }
}