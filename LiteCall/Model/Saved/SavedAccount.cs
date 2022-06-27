using LiteCall.Model.Users;

namespace LiteCall.Model.Saved;

internal class SavedMainAccount
{
    public Account? MainAccount { get; set; }

    public Settings? Settings { get; set; }
}