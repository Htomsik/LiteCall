using Core.Models.Users;

namespace Core.Models.Saved;

public sealed class CurrentAccountSavedServers : AppSavedServers
{
    public Account? MainServerAccount { get; set; } = null;
}