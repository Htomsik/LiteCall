using System.Net;
using System.Threading.Tasks;
using LiteCall.Model.RegistrationRecovery;
using LiteCall.Model.Users;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.Registration;

internal sealed class RegistrationApiServerServices : IRegistrationServices
{
    private readonly INavigationService _closeModalNavigationService;

    private readonly CurrentServerStore _currentServerStore;


    private readonly IHttpDataServices _httpDataServices;

    private readonly SavedServersStore _savedServersStore;


    public RegistrationApiServerServices(SavedServersStore savedServersStore, CurrentServerStore currentServerStore,
        INavigationService closeModalNavigationService, IHttpDataServices httpDataServices)
    {
        _closeModalNavigationService = closeModalNavigationService;

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

        _closeModalNavigationService.Navigate();

        return 2;
    }
}