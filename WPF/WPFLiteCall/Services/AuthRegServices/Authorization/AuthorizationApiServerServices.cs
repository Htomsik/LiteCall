using System.Threading.Tasks;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Authorization;

internal sealed class AuthorizationApiServerSc : IAuthorizationSc
{
    private readonly INavigationSc _closeModalNavigationSc;

    private readonly CurrentServerStore _currentServerStore;

    private readonly IHttpDataServices _httpDataServices;

    private readonly SavedServersStore _savedServersStore;

    public AuthorizationApiServerSc(SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore, INavigationSc closeModalNavigationSc,
        IHttpDataServices httpDataServices)
    {
        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;

        _closeModalNavigationSc = closeModalNavigationSc;

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

        _closeModalNavigationSc.Navigate();

        return 1;
    }
}