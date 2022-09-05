using Newtonsoft.Json;
using ReactiveUI;

namespace Core.Models.Users;

public class RegistrationUser:ReactiveObject, IEquatable<RegistrationUser>
{
    [JsonProperty]
    public string? Login { get; set; }
    
    public string? Password { get; set; }


    public bool Equals(RegistrationUser? other)
    {
        if (ReferenceEquals(null, other)) return false;
        
        if (ReferenceEquals(this, other)) return true;
        
        return Login == other.Login && Password == other.Password;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        
        if (ReferenceEquals(this, obj)) return true;
        
        if (obj.GetType() != this.GetType()) return false;
        
        return Equals((RegistrationUser)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Login, Password);
    }
}