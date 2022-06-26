using LiteCall.Model.Users;

namespace LiteCall.Model.Saved;

internal sealed class SavedServers : AppSavedServers
{
    public Account? MainServerAccount { get; set; } = null;
}