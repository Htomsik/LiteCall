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

        public async Task<int> Registration(Account _NewAccount,string _Captcha)
        {
            
            var Response = await _httpDataServices.Registration(_NewAccount,_Captcha);

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

            _mainAccountStore.CurrentAccount = _NewAccount;

            return 2;

        }
    }
}
