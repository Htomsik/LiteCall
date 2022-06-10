using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;

namespace LiteCall.Services
{
    internal class RegistrationApiServerServices:IRegistrationSevices
    {

        private ServersAccountsStore _ServersAccountsStore;

        private readonly CurrentServerStore _CurrentServerStore;

        private readonly INavigationService _CloseModalNavigationService;


        public RegistrationApiServerServices(ServersAccountsStore serversAccountsStore, CurrentServerStore currentServerStore, INavigationService closeModalNavigationService)
        {
            _CloseModalNavigationService = closeModalNavigationService;

            _ServersAccountsStore = serversAccountsStore;

            _CurrentServerStore = currentServerStore;
        }

        public async Task<int> Registration(Account _NewAccount, string _Captcha)
        {
            var Response = await DataBaseService.Registration(_NewAccount, _Captcha,_CurrentServerStore.CurrentServer.ApiIp);

            if (Response.Replace(" ", "") == System.Net.HttpStatusCode.BadRequest.ToString())
            {
                return 0;
            }
            else if (Response == System.Net.HttpStatusCode.Conflict.ToString())
            {
                return 1;
            }

            _NewAccount.Token = Response;

            _NewAccount.IsAuthorise = true;

            _ServersAccountsStore.replace(_CurrentServerStore.CurrentServer,_NewAccount);

            _CloseModalNavigationService.Navigate();

            return 2;
        }
    }
}
