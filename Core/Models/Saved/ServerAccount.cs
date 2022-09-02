using Core.Models.Servers;
using Core.Models.Users;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Saved;

public  sealed class ServerAccount : ReactiveObject
{
    [Reactive]
    public Server? SavedServer { get; set; }
    [Reactive]
    public Account? Account { get; set; }
}