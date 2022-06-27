using LiteCall.Model.ServerModels;
using LiteCall.Model.Users;

namespace LiteCall.Model.Saved;

internal sealed class ServerAccount
{
    public Server? SavedServer { get; set; }

    public Account? Account { get; set; }
}