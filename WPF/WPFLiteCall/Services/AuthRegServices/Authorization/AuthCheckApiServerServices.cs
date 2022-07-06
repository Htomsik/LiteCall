using System.Threading.Tasks;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Authorization;

internal sealed class AuthCheckApiServerSc : IAuthorizationSc
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly CurrentServerAccountStore _currentServerAccountStore;

    public AuthCheckApiServerSc(CurrentServerAccountStore currentServerAccountStore, IHttpDataServices httpDataServices)
    {
        _currentServerAccountStore = currentServerAccountStore;

        _httpDataServices = httpDataServices;
    }

    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? ApiServerIp)
    {
        if (isNotAnonymousAuthorize)
        {
            var response = await _httpDataServices.GetAuthorizeToken(newAccount, ApiServerIp);

            if (response == "invalid") return 0;

            newAccount!.Role = await _httpDataServices.GetRoleFromJwtToken(response);

            newAccount.Token = response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount!.IsAuthorized = false;

            newAccount.Password = "";

            var response = await _httpDataServices.GetAuthorizeToken(newAccount, ApiServerIp);


            if (response == "invalid") return 0;

            newAccount.Role = await _httpDataServices.GetRoleFromJwtToken(response);

            newAccount.Token = response;
        }

        _currentServerAccountStore.CurrentAccount = newAccount;

        return 1;
    }
}