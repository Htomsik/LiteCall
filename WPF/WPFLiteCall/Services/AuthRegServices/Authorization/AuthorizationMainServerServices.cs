using System.Threading.Tasks;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Authorization;

internal sealed class AuthorizationMainServerSc : IAuthorizationSc
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly MainAccountStore _mainAccountStore;

    private readonly IStatusSc _statusSc;

    public AuthorizationMainServerSc(MainAccountStore accountStore, IHttpDataServices httpDataServices,
        IStatusSc statusSc)
    {
        _mainAccountStore = accountStore;

        _httpDataServices = httpDataServices;

        _statusSc = statusSc;
    }

    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? severIp = null)
    {
        if (isNotAnonymousAuthorize)
        {
            var response = await _httpDataServices.GetAuthorizeToken(newAccount);

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