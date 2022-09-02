using System.Net;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Registration;

public sealed class RegistrationMainServerSc : IRegistrationSc
{
    private readonly IHttpDataSc _httpDataSc;

    private readonly MainAccountStore _mainAccountStore;

    public RegistrationMainServerSc(MainAccountStore mainAccountStore, IHttpDataSc httpDataSc)
    {
        _mainAccountStore = mainAccountStore;

        _httpDataSc = httpDataSc;
    }

    public async Task<int> Registration(RegistrationModel registrationModel)
    {
        var response = await _httpDataSc.Registration(registrationModel);

        if (response.Replace(" ", "") == HttpStatusCode.BadRequest.ToString())
            return 0;
        if (response == HttpStatusCode.Conflict.ToString()) return 1;

        var newAccount = (Account)registrationModel.RecoveryAccount!;

        newAccount.Token = response;

        newAccount.IsAuthorized = true;

        _mainAccountStore.CurrentValue = newAccount;


        return 2;
    }
}