using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;

namespace LiteCall.Services
{
    internal class AuthorisationApiServerServices : IAuthorisationServices
    {

        private ServersAccountsStore _ServersAccountsStore;

        private readonly CurrentServerStore _CurrentServerStore;

        private readonly INavigationService _CloseModalNavigationService;

        public AuthorisationApiServerServices(ServersAccountsStore serversAccountsStore,
            CurrentServerStore currentServerStore, INavigationService closeModalNavigationService)
        {
            _ServersAccountsStore = serversAccountsStore;

            _CurrentServerStore = currentServerStore;

            _CloseModalNavigationService = closeModalNavigationService;

        }


        public async Task<int> Login(bool isSeverAuthorise, Account _NewAccount, string ApiSeverIp)
        {
            if (isSeverAuthorise)
            {
                var Response =
                    await DataBaseService.GetAuthorizeToken(_NewAccount, _CurrentServerStore.CurrentServer.ApiIp);

                if (Response == "invalid")
                {
                    return 0;
                }

                _NewAccount.Token = Response;

                _NewAccount.IsAuthorise = true;
            }
            else
            {
                _NewAccount.IsAuthorise = false;

                _NewAccount.Password = "";

                var Response = await DataBaseService.GetAuthorizeToken(_NewAccount);

                if (Response == "invalid")
                {
                    MessageBox.Show("The server is not responding. The account will be local", "Сообщение");
                }
                else
                {
                    _NewAccount.Token = Response;
                }

            }

            _ServersAccountsStore.replace(_CurrentServerStore.CurrentServer, _NewAccount);

            _CloseModalNavigationService.Navigate();

            return 1;
        }

     

    }
}
