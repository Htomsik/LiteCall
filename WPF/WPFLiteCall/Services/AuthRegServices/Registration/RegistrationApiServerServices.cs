using System.Net;
using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Registration;

internal sealed class RegistrationApiServerSc : IRegistrationSc
{
    private readonly INavigationSc _closeModalNavigationSc;

    private readonly CurrentServerStore _currentServerStore;


    private readonly IHttpDataServices _httpDataServices;

    private readonly SavedServersStore _savedServersStore;


    public RegistrationApiServerSc(SavedServersStore savedServersStore, CurrentServerStore currentServerStore,
        INavigationSc closeModalNavigationSc, IHttpDataServices httpDataServices)
    {
        _closeModalNavigationSc = closeModalNavigationSc;

        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;

        _httpDataServices = httpDataServices;
    }

    public async Task<int> Registration(RegistrationModel registrationModel)
    {
        var response =
            await _httpDataServices.Registration(registrationModel, _currentServerStore.CurrentServer!.ApiIp);

        if (response.Replace(" ", "") == HttpStatusCode.BadRequest.ToString())
            return 0; //если не верна капча
        if (response == HttpStatusCode.Conflict.ToString())
            return 1; // если неверные данные регистрации

        var newAccount = (Account)registrationModel.RecoveryAccount!;

        newAccount.Token = response;

        newAccount.IsAuthorized = true;

        _savedServersStore.Replace(_currentServerStore.CurrentServer, newAccount);

        _closeModalNavigationSc.Navigate();

        return 2;
    }
}