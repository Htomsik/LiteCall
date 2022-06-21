using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using Exception = System.Exception;

namespace LiteCall.Services
{
    internal class SynchronizeDataOnServerServices:ISynhronyzeDataOnServerServices
    {
        private readonly AccountStore _accountStore;

        private readonly SavedServersStore _savedServersStore;

        private readonly IhttpDataServices _httpDataServices;

        private readonly IEncryptServices _encryptServices;

        public SynchronizeDataOnServerServices(AccountStore accountStore, SavedServersStore savedServersStore,IhttpDataServices httpDataServices,IEncryptServices encryptServices)
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
            {
                return false;
            }


            var savedServers = _savedServersStore.SavedServerAccounts;

                foreach (var elem in savedServers.ServersAccounts)
                {
                    elem.Account.Password = await _encryptServices.Base64Decrypt(elem.Account.Password);
                }

            return await _httpDataServices.PostSaveServersUserOnMainServer(_accountStore.CurrentAccount, savedServers);

        }


        public async Task<bool> GetFromServer()
        {
            if (string.IsNullOrEmpty(_accountStore?.CurrentAccount?.Password))
            {
                return false;
            }

            AppSavedServers? dataFromServer = await _httpDataServices.GetSaveServersUserOnMainServer(_accountStore.CurrentAccount,
                _savedServersStore.SavedServerAccounts?.LastUpdated);

            if (dataFromServer != null)
            {
                _savedServersStore.SavedServerAccounts = dataFromServer;
                return true;
            }
           

            return false;
        }
    }
}
