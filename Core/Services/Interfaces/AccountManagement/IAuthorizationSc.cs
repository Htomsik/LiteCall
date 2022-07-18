using Core.Models.Users;

namespace Core.Services.Interfaces.AccountManagement;

public interface IAuthorizationSc
{
    Task Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiServerIp = null);
    
}