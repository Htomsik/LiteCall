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

    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? ApiServerIp)
    {
        if (isNotAnonymousAuthorize)
        {
            var response = await _httpDataSc.GetAuthorizeToken(newAccount, ApiServerIp);

            if (response == "invalid") return 0;

            newAccount!.Role = await _httpDataSc.GetRoleFromJwtToken(response);

            newAccount.Token = response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount!.IsAuthorized = false;

            newAccount.Password = "";

            var response = await _httpDataSc.GetAuthorizeToken(newAccount, ApiServerIp);


            if (response == "invalid") return 0;

            newAccount.Role = await _httpDataSc.GetRoleFromJwtToken(response);

            newAccount.Token = response;
        }

        _currentServerAccountStore.CurrentAccount = newAccount;

        return 1;
    }
}