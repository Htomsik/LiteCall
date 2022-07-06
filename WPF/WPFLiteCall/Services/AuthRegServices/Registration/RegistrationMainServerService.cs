using System.Net;
using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.Registration;

internal sealed class RegistrationMainServerSc : IRegistrationSc
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly MainAccountStore _mainAccountStore;

    public RegistrationMainServerSc(MainAccountStore mainAccountStore, IHttpDataServices httpDataServices)
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