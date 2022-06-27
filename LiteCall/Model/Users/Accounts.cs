namespace LiteCall.Model.Users;

public class Account : RegRecPasswordAccount
{
    public string? CurrentServerLogin { get; set; }

    public string? Role { get; set; }

    public string? Token { get; set; }

    public bool IsAuthorized { get; set; }
}