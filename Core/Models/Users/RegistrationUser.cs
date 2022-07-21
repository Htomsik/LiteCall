using ReactiveUI;

namespace Core.Models.Users;

public class RegistrationUser:ReactiveObject
{
    public string? Login { get; set; }
    
    public string? Password { get; set; }
}