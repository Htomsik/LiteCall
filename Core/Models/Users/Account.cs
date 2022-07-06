namespace Core.Models.Users;

public class Account:RegistrationUser
{

    public string? CurrentServerLogin { get; set; }

    public string? Role { get; set; }

    public string? Token { get; set; }

    public bool IsAuthorized { get; set; }
}