using Core.Models.Servers;
using Core.Models.Users;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Saved;

public sealed class ServerAccount : ReactiveObject
{
    [Reactive]
    [JsonProperty]
    public Server? SavedServer { get; set; }
    [Reactive]
    [JsonProperty]
    public Account? Account { get; set; }

    #region Constructors

    public ServerAccount(){}

    public ServerAccount(Server savedServer, Account account)
    {
        SavedServer = savedServer;
        Account = account;
    }
   

    #endregion
   
    
    
}