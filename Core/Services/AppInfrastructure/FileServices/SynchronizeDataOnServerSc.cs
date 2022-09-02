using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Services.Interfaces.Extra;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AppInfrastructure.FileServices;

public sealed class SynchronizeDataOnServerSc : ISyncDataOnServerSc
{
    private readonly MainAccountStore _accountStore;

    private readonly IEncryptSc _encryptSc;

    private readonly IHttpDataSc _httpDataSc;

    private readonly SavedServersStore _savedServersStore;

    public SynchronizeDataOnServerSc(MainAccountStore accountStore, SavedServersStore savedServersStore,
        IHttpDataSc httpDataSc, IEncryptSc encryptSc)
    {
        _accountStore = accountStore;

        _savedServersStore = savedServersStore;

        _httpDataSc = httpDataSc;

        _encryptSc = encryptSc;
    }

    public async Task<bool> SaveOnServer()
    {
        if (string.IsNullOrEmpty(_accountStore?.CurrentValue?.Password) ||
            _savedServersStore?.SavedServerAccounts?.ServersAccounts is null)
            return false;


        var savedServers = _savedServersStore.SavedServerAccounts;

        foreach (var elem in savedServers.ServersAccounts)
            elem.Account!.Password = await _encryptSc.Base64Decrypt(elem.Account.Password);

        return await _httpDataSc.PostSaveServersUserOnMainServer(_accountStore.CurrentValue, savedServers);
    }


    public async Task<bool> GetFromServer()
    {
        if (string.IsNullOrEmpty(_accountStore?.CurrentValue?.Password)) return false;

        var dataFromServer = await _httpDataSc.GetSaveServersUserOnMainServer(_accountStore.CurrentValue,
            _savedServersStore.SavedServerAccounts?.LastUpdated);

        if (dataFromServer != null)
        {
            _savedServersStore.SavedServerAccounts = dataFromServer;
            return true;
        }


        return false;
    }
}