using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Authorization;

public sealed class AuthCheckApiServerSc : IAuthorizationSc
{
    private readonly IHttpDataSc _httpDataSc;

    private readonly CurrentServerAccountStore _currentServerAccountStore;

    public AuthCheckApiServerSc(CurrentServerAccountStore currentServerAccountStore, IHttpDataSc httpDataSc)
    {
        _currentServerAccountStore = currentServerAccountStore;

        _httpDataSc = httpDataSc;
    }

    public async Task Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiServerIp)
    {
        newAccount!.IsAuthorized = isNotAnonymousAuthorize;
        
        newAccount.Password = isNotAnonymousAuthorize ? string.Empty : newAccount.Password;

        try
        {
            var tokenResponse = await _httpDataSc.GetAuthorizeToken(newAccount, apiServerIp);
        
            newAccount.Role = await _httpDataSc.GetRoleFromJwtToken(tokenResponse);
        
            newAccount.Token = tokenResponse;

            _currentServerAccountStore.CurrentAccount = newAccount;
        }
        catch (Exception)
        {
            throw new Exception();
        }
        
    }
}