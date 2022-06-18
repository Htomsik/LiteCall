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

        private readonly ServersAccountsStore _serversAccountsStore;

        private readonly CurrentServerStore _currentServerStore;

        private readonly INavigationService _closeModalNavigationService;


        private readonly IhttpDataServices _httpDataServices;


        public RegistrationApiServerServices(ServersAccountsStore serversAccountsStore, CurrentServerStore currentServerStore, INavigationService closeModalNavigationService, IhttpDataServices httpDataServices)
        {
            _closeModalNavigationService = closeModalNavigationService;

            _serversAccountsStore = serversAccountsStore;

            _currentServerStore = currentServerStore;

            _httpDataServices = httpDataServices;
        }

        public async Task<int> Registration(Account _NewAccount, string _Captcha)
        {
            var Response = await _httpDataServices.Registration(_NewAccount, _Captcha,_currentServerStore.CurrentServer.ApiIp);

            if (Response.Replace(" ", "") == System.Net.HttpStatusCode.BadRequest.ToString())
            {
                //если не верна капча
                return 0;
            }
            else if (Response == System.Net.HttpStatusCode.Conflict.ToString())
            {
                // если неверные данные регистрации
                return 1;
            }

            _NewAccount.Token = Response;

            _NewAccount.IsAuthorise = true;

            _serversAccountsStore.replace(_currentServerStore.CurrentServer,_NewAccount);

            _closeModalNavigationService.Navigate();

            return 2;
        }
    }
}
