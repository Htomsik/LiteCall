using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services;

internal sealed class SynchronizeDataOnServerServices : ISynhronyzeDataOnServerServices
{
    private readonly AccountStore _accountStore;

    private readonly IEncryptServices _encryptServices;

    private readonly IHttpDataServices _httpDataServices;

    private readonly SavedServersStore _savedServersStore;

    public SynchronizeDataOnServerServices(AccountStore accountStore, SavedServersStore savedServersStore,
        IHttpDataServices httpDataServices, IEncryptServices encryptServices)
    {
        _accountStore = accountStore;

        _savedServersStore = savedServersStore;

        _httpDataServices = httpDataServices;

        _encryptServices = encryptServices;
    }

    public async Task<bool> SaveOnServer()
    {
        if (string.IsNullOrEmpty(_accountStore?.CurrentAccount?.Password) ||
            _savedServersStore?.SavedServerAccounts?.ServersAccounts is null)
            return false;


        var savedServers = _savedServersStore.SavedServerAccounts;

        foreach (var elem in savedServers.ServersAccounts)
            elem.Account!.Password = await _encryptServices.Base64Decrypt(elem.Account.Password);

        return await _httpDataServices.PostSaveServersUserOnMainServer(_accountStore.CurrentAccount, savedServers);
    }


    public async Task<bool> GetFromServer()
    {
        if (string.IsNullOrEmpty(_accountStore?.CurrentAccount?.Password)) return false;

        var dataFromServer = await _httpDataServices.GetSaveServersUserOnMainServer(_accountStore.CurrentAccount,
            _savedServersStore.SavedServerAccounts?.LastUpdated);

        if (dataFromServer != null)
        {
            _savedServersStore.SavedServerAccounts = dataFromServer;
            return true;
        }


        return false;
    }
}