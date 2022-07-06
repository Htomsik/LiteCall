using Core.Models.Users;

namespace Core.Services.Interfaces.AccountManagement;

public interface IAuthorizationSc
{
    Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiServerIp = null);
}