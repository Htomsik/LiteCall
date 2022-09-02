using System.Net;
using AppInfrastructure.Services.NavigationServices.Close;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Registration;

public sealed class RegistrationApiServerSc : IRegistrationSc
{
    private readonly ICloseServices _closeModalNavigationServices;

    private readonly CurrentServerStore _currentServerStore;
    
    private readonly IHttpDataSc _httpDataSc;

    private readonly SavedServersStore _savedServersStore;


    public RegistrationApiServerSc(SavedServersStore savedServersStore, CurrentServerStore currentServerStore,
        ICloseServices closeModalNavigationServices, IHttpDataSc httpDataSc)
    {
        _closeModalNavigationServices = closeModalNavigationServices;

        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;

        _httpDataSc = httpDataSc;
    }

    public async Task<int> Registration(RegistrationModel registrationModel)
    {
        var response =
            await _httpDataSc.Registration(registrationModel, _currentServerStore.CurrentServer!.ApiIp);

        if (response.Replace(" ", "") == HttpStatusCode.BadRequest.ToString())
            return 0; //если не верна капча
        if (response == HttpStatusCode.Conflict.ToString())
            return 1; // если неверные данные регистрации

        var newAccount = (Account)registrationModel.RecoveryAccount!;

        newAccount.Token = response;

        newAccount.IsAuthorized = true;

        _savedServersStore.Replace(_currentServerStore.CurrentServer, newAccount);

        _closeModalNavigationServices.Close();

        return 2;
    }
}