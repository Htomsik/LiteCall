using Core.Stores.TemporaryInfo;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Users;

public class Account:RegistrationUser
{
    
   [Reactive]
    public string? CurrentServerLogin { get; set; }
  
    [Reactive]
    public string? Role { get; set; }

    [Reactive]
    public string? Token { get; set; }

    [Reactive]
    public bool IsAuthorized { get; set; }
}