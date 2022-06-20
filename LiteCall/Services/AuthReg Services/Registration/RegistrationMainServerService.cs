using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal class RegistrationMainServerService:IRegistrationSevices
    {

        private readonly AccountStore _mainAccountStore;

        private readonly IhttpDataServices _httpDataServices;

        public RegistrationMainServerService(AccountStore mainAccountStore, IhttpDataServices httpDataServices)
        {
            _mainAccountStore = mainAccountStore;

            _httpDataServices = httpDataServices;
        }

        public async Task<int> Registration( RegistrationModel registrationModel)
        {
            
            var response = await _httpDataServices.Registration(registrationModel);

            if (response.Replace(" ", "") == System.Net.HttpStatusCode.BadRequest.ToString())
            {
                return 0;
            }
            else if (response == System.Net.HttpStatusCode.Conflict.ToString())
            {
                return 1;
            }

            Account newAccount = (Account)registrationModel.recoveryAccount;

            newAccount.Token = response;

            newAccount.IsAuthorise = true;

            _mainAccountStore.CurrentAccount = newAccount;


            return 2;

        }

      
    }
}
