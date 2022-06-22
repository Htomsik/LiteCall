using System.Net;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.Registration;

internal class RegistrationMainServerService : IRegistrationServices
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly AccountStore _mainAccountStore;

    public RegistrationMainServerService(AccountStore mainAccountStore, IHttpDataServices httpDataServices)
    {
        _mainAccountStore = mainAccountStore;

        _httpDataServices = httpDataServices;
    }

    public async Task<int> Registration(RegistrationModel registrationModel)
    {
        var response = await _httpDataServices.Registration(registrationModel);

        if (response.Replace(" ", "") == HttpStatusCode.BadRequest.ToString())
            return 0;
        if (response == HttpStatusCode.Conflict.ToString()) return 1;

        var newAccount = (Account)registrationModel.RecoveryAccount!;

        newAccount.Token = response;

        newAccount.IsAuthorized = true;

        _mainAccountStore.CurrentAccount = newAccount;


        return 2;
    }
}