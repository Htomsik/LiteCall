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
using Microsoft.VisualBasic.ApplicationServices;

namespace LiteCall.Services
{
    internal class AuthorisationApiServerServices : IAuthorisationServices
    {

        private SavedServersStore _savedServersStore;

        private readonly CurrentServerStore _CurrentServerStore;

        private readonly INavigationService _CloseModalNavigationService;

        private readonly IhttpDataServices _HttpDataServices;

        public AuthorisationApiServerServices(SavedServersStore savedServersStore,
            CurrentServerStore currentServerStore, INavigationService closeModalNavigationService, IhttpDataServices httpDataServices)
        {
            _savedServersStore = savedServersStore;

            _CurrentServerStore = currentServerStore;

            _CloseModalNavigationService = closeModalNavigationService;

            _HttpDataServices = httpDataServices;

        }


        public async Task<int> Login(bool isSeverAuthorise, Account _NewAccount, string ApiSeverIp)
        {
            if (isSeverAuthorise)
            {
                var Response =
                    await _HttpDataServices.GetAuthorizeToken(_NewAccount, _CurrentServerStore.CurrentServer.ApiIp);

                if (Response == "invalid")
                {
                    return 0;
                }

                _NewAccount.Role = await _HttpDataServices.GetRoleFromJwtToken(Response);

                _NewAccount.Token = Response;

                _NewAccount.IsAuthorise = true;
            }
            else
            {
                _NewAccount.IsAuthorise = false;

                _NewAccount.Password = "";

                var Response = await _HttpDataServices.GetAuthorizeToken(_NewAccount,_CurrentServerStore.CurrentServer.ApiIp);

                if (Response == "invalid")
                {
                    return 0;
                }
                else
                {
                    _NewAccount.Role = await _HttpDataServices.GetRoleFromJwtToken(Response);

                    _NewAccount.Token = Response;
                }

            }



            _savedServersStore.Replace(_CurrentServerStore.CurrentServer, _NewAccount);

            _CloseModalNavigationService.Navigate();

            return 1;
        }

     

    }
}
