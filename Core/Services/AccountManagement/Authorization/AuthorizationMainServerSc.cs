using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Authorization;

public sealed class AuthorizationMainServerSc : IAuthorizationSc
{
    private readonly IHttpDataSc _httpDataSc;

    private readonly MainAccountStore _mainAccountStore;

    private readonly IStatusSc _statusSc;

    public AuthorizationMainServerSc(MainAccountStore accountStore, IHttpDataSc httpDataSc,
        IStatusSc statusSc)
    {
        _mainAccountStore = accountStore;

        _httpDataSc = httpDataSc;

        _statusSc = statusSc;
    }

    public async Task Login(bool isNotAnonymousAuthorize, Account? newAccount, string? severIp = null)
    {
        newAccount!.IsAuthorized = isNotAnonymousAuthorize;
        
        newAccount.Password = isNotAnonymousAuthorize ? string.Empty : newAccount.Password;

        try
        {
            newAccount!.Token = isNotAnonymousAuthorize ? await _httpDataSc.GetAuthorizeToken(newAccount) : null;
        }
        catch (Exception)
        {
            throw new Exception();
        }
        
        _mainAccountStore.CurrentAccount = newAccount;
        
    }
}