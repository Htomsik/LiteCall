using Core.Models.Servers;
using Core.Models.Users;

namespace Core.Models.Saved;

public  sealed class ServerAccount
{
    public Server? SavedServer { get; set; }

    public Account? Account { get; set; }
}