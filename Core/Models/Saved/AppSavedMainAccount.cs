using Core.Models.AppInfrastructure;
using Core.Models.Users;

namespace Core.Models.Saved;

public class AppSavedMainAccount
{
    public Account? MainAccount { get; set; }

    public AppSettings? Settings { get; set; }
}