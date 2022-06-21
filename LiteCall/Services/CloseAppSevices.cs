using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal class CloseAppSevices:ICloseAppSevices
    {
        private readonly IhttpDataServices _httpDataServices;

        private readonly AccountStore _accountStore;

        private readonly ServersAccountsStore _serversAccountsStore;

        public CloseAppSevices(IhttpDataServices httpDataServices,AccountStore accountStore, ServersAccountsStore serversAccountsStore)
        {
            _httpDataServices = httpDataServices;

            _accountStore = accountStore;

            _serversAccountsStore = serversAccountsStore;
        }
        public async Task Close()
        {

            await _httpDataServices.PostSaveServersUserOnMainServer(_accountStore.CurrentAccount,
                _serversAccountsStore.SavedServerAccounts);

            Application.Current.Shutdown();
        }
    }
}
