using LiteCall.ViewModels.Base;

namespace LiteCall.Model;

public class User
{
    public string? Login { get; set; }
}

public class ServerUser : User
{
    public string? Role { get; set; }
}

public class RegRecPasswordAccount : User
{
    public string? Password { get; set; }
}

public class Account : RegRecPasswordAccount
{
    public string? CurrentServerLogin { get; set; }

    public string? Role { get; set; }

    public string? Token { get; set; }

    public bool IsAuthorized { get; set; }
}