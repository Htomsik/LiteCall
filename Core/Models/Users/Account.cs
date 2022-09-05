using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Users;

public class Account : RegistrationUser, IEquatable<Account>
{
    [Reactive] [JsonIgnore] public string? CurrentServerLogin { get; set; }

    [Reactive] [JsonIgnore] public string? Role { get; set; }

    [Reactive] [JsonIgnore] public string? Token { get; set; }

    [Reactive] [JsonProperty] public bool IsAuthorized { get; set; }

    #region IEquatable

    public bool Equals(Account? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && CurrentServerLogin == other.CurrentServerLogin && Role == other.Role && Token == other.Token && IsAuthorized == other.IsAuthorized;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Account)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), CurrentServerLogin, Role, Token, IsAuthorized);
    }

    #endregion
 
}