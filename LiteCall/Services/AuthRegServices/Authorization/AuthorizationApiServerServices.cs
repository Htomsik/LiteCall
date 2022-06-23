using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.Authorization;

internal class AuthorizationApiServerServices : IAuthorizationServices
{
    private readonly INavigationService _closeModalNavigationService;

    private readonly CurrentServerStore _currentServerStore;

    private readonly IHttpDataServices _httpDataServices;

    private readonly SavedServersStore _savedServersStore;

    public AuthorizationApiServerServices(SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore, INavigationService closeModalNavigationService,
        IHttpDataServices httpDataServices)
    {
        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;

        _closeModalNavigationService = closeModalNavigationService;

        _httpDataServices = httpDataServices;
    }


    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiSeverIp)
    {
        if (isNotAnonymousAuthorize)
        {
            var response =
                await _httpDataServices.GetAuthorizeToken(newAccount, _currentServerStore.CurrentServer!.ApiIp);

            if (response == "invalid") return 0;

            newAccount!.Role = await _httpDataServices.GetRoleFromJwtToken(response);

            newAccount.Token = response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount!.IsAuthorized = false;

            newAccount.Password = "";

            var response =
                await _httpDataServices.GetAuthorizeToken(newAccount, _currentServerStore.CurrentServer!.ApiIp);

            if (response == "invalid") return 0;

            newAccount.Role = await _httpDataServices.GetRoleFromJwtToken(response);

            newAccount.Token = response;
        }


        _savedServersStore.Replace(_currentServerStore.CurrentServer, newAccount);

        _closeModalNavigationService.Navigate();

        return 1;
    }
}