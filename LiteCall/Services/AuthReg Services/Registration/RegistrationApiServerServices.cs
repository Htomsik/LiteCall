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

        private readonly SavedServersStore _savedServersStore;

        private readonly CurrentServerStore _currentServerStore;

        private readonly INavigationService _closeModalNavigationService;


        private readonly IhttpDataServices _httpDataServices;


        public RegistrationApiServerServices(SavedServersStore savedServersStore, CurrentServerStore currentServerStore, INavigationService closeModalNavigationService, IhttpDataServices httpDataServices)
        {
            _closeModalNavigationService = closeModalNavigationService;

            _savedServersStore = savedServersStore;

            _currentServerStore = currentServerStore;

            _httpDataServices = httpDataServices;
        }

        public async Task<int> Registration( RegistrationModel registrationModel)
        {
            var Response = await _httpDataServices.Registration(registrationModel, _currentServerStore.CurrentServer.ApiIp);

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

            Account newAccount = (Account)registrationModel.recoveryAccount;

            newAccount.Token = Response;

            newAccount.IsAuthorise = true;

            _savedServersStore.Replace(_currentServerStore.CurrentServer,newAccount);

            _closeModalNavigationService.Navigate();

            return 2;
        }
    }
}
