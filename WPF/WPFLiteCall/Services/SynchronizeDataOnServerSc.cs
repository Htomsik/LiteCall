using System.Threading.Tasks;
using Core.Services;
using Core.Services.Interfaces.Extra;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services;

internal sealed class SynchronizeDataOnServerSc : ISyncDataOnServerSc
{
    private readonly MainAccountStore _accountStore;

    private readonly IEncryptSc _encryptSc;

    private readonly IHttpDataServices _httpDataServices;

    private readonly SavedServersStore _savedServersStore;

    public SynchronizeDataOnServerSc(MainAccountStore accountStore, SavedServersStore savedServersStore,
        IHttpDataServices httpDataServices, IEncryptSc encryptSc)
    {
        _accountStore = accountStore;

        _savedServersStore = savedServersStore;

        _httpDataServices = httpDataServices;

        _encryptSc = encryptSc;
    }

    public async Task<bool> SaveOnServer()
    {
        if (string.IsNullOrEmpty(_accountStore?.CurrentAccount?.Password) ||
            _savedServersStore?.SavedServerAccounts?.ServersAccounts is null)
            return false;


        var savedServers = _savedServersStore.SavedServerAccounts;

        foreach (var elem in savedServers.ServersAccounts)
            elem.Account!.Password = await _encryptSc.Base64Decrypt(elem.Account.Password);

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