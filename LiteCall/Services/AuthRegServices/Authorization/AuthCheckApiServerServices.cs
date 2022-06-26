using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Model.Users;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.Authorization;

internal sealed class AuthCheckApiServerServices : IAuthorizationServices
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly ServerAccountStore _serverAccountStore;

    public AuthCheckApiServerServices(ServerAccountStore serverAccountStore, IHttpDataServices httpDataServices)
    {
        _serverAccountStore = serverAccountStore;

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

        _serverAccountStore.CurrentAccount = newAccount;

        return 1;
    }
}