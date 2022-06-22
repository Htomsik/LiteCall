using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services;

internal class AuthoisationMainServerServices : IAuthorizationServices
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly AccountStore _mainAccountStore;

    private readonly IStatusServices _statusServices;

    public AuthoisationMainServerServices(AccountStore accountStore, IHttpDataServices httpDataServices,
        IStatusServices statusServices)
    {
        _mainAccountStore = accountStore;

        _httpDataServices = httpDataServices;

        _statusServices = statusServices;
    }

    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? severIp = null)
    {
        if (isNotAnonymousAuthorize)
        {
            var Response = await _httpDataServices.GetAuthorizeToken(newAccount);

            if (Response == "invalid") return 0;

            newAccount.Token = Response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount.IsAuthorized = false;

            newAccount.Password = "";
        }

        _mainAccountStore.CurrentAccount = newAccount;

        return 1;
    }
}