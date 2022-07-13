using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Authorization;

public sealed class AuthorizationMainServerSc : IAuthorizationSc
{
    private readonly IHttpDataSc _httpDataSc;

    private readonly MainAccountStore _mainAccountStore;

    private readonly IStatusSc _statusSc;

    public AuthorizationMainServerSc(MainAccountStore accountStore, IHttpDataSc httpDataSc,
        IStatusSc statusSc)
    {
        _mainAccountStore = accountStore;

        _httpDataSc = httpDataSc;

        _statusSc = statusSc;
    }

    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? severIp = null)
    {
        if (isNotAnonymousAuthorize)
        {
            var response = await _httpDataSc.GetAuthorizeToken(newAccount);

            if (response == "invalid") return 0;

            newAccount!.Token = response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount!.IsAuthorized = false;

            newAccount.Password = "";
        }

        _mainAccountStore.CurrentAccount = newAccount;

        return 1;
    }
}