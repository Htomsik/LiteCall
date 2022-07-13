using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Authorization;

public sealed class AuthorizationApiServerSc : IAuthorizationSc
{
    private readonly INavigationSc _closeModalNavigationSc;

    private readonly CurrentServerStore _currentServerStore;

    private readonly IHttpDataSc _httpDataSc;

    private readonly SavedServersStore _savedServersStore;

    public AuthorizationApiServerSc(SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore, INavigationSc closeModalNavigationSc,
        IHttpDataSc httpDataSc)
    {
        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;

        _closeModalNavigationSc = closeModalNavigationSc;

        _httpDataSc = httpDataSc;
    }


    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiSeverIp)
    {
        if (isNotAnonymousAuthorize)
        {
            var response =
                await _httpDataSc.GetAuthorizeToken(newAccount, _currentServerStore.CurrentServer!.ApiIp);

            if (response == "invalid") return 0;

            newAccount!.Role = await _httpDataSc.GetRoleFromJwtToken(response);

            newAccount.Token = response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount!.IsAuthorized = false;

            newAccount.Password = "";

            var response =
                await _httpDataSc.GetAuthorizeToken(newAccount, _currentServerStore.CurrentServer!.ApiIp);

            if (response == "invalid") return 0;

            newAccount.Role = await _httpDataSc.GetRoleFromJwtToken(response);

            newAccount.Token = response;
        }


        _savedServersStore.Replace(_currentServerStore.CurrentServer, newAccount);

        _closeModalNavigationSc.Navigate();

        return 1;
    }
}