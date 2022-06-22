using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services;

internal class AuthorizationApiServerServices : IAuthorizationServices
{
    private readonly INavigationService _CloseModalNavigationService;

    private readonly CurrentServerStore _CurrentServerStore;

    private readonly IHttpDataServices _HttpDataServices;

    private readonly SavedServersStore _savedServersStore;

    public AuthorizationApiServerServices(SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore, INavigationService closeModalNavigationService,
        IHttpDataServices httpDataServices)
    {
        _savedServersStore = savedServersStore;

        _CurrentServerStore = currentServerStore;

        _CloseModalNavigationService = closeModalNavigationService;

        _HttpDataServices = httpDataServices;
    }


    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? _NewAccount, string? ApiSeverIp)
    {
        if (isNotAnonymousAuthorize)
        {
            var Response =
                await _HttpDataServices.GetAuthorizeToken(_NewAccount, _CurrentServerStore.CurrentServer.ApiIp);

            if (Response == "invalid") return 0;

            _NewAccount.Role = await _HttpDataServices.GetRoleFromJwtToken(Response);

            _NewAccount.Token = Response;

            _NewAccount.IsAuthorized = true;
        }
        else
        {
            _NewAccount.IsAuthorized = false;

            _NewAccount.Password = "";

            var Response =
                await _HttpDataServices.GetAuthorizeToken(_NewAccount, _CurrentServerStore.CurrentServer.ApiIp);

            if (Response == "invalid") return 0;

            _NewAccount.Role = await _HttpDataServices.GetRoleFromJwtToken(Response);

            _NewAccount.Token = Response;
        }


        _savedServersStore.Replace(_CurrentServerStore.CurrentServer, _NewAccount);

        _CloseModalNavigationService.Navigate();

        return 1;
    }
}