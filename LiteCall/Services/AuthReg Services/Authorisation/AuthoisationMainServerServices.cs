using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal class AuthoisationMainServerServices:IAuthorisationServices
    {

        private readonly AccountStore _mainAccountStore;

        private readonly IhttpDataServices _httpDataServices;

        private readonly IStatusServices _statusServices;

        public AuthoisationMainServerServices(AccountStore accountStore, IhttpDataServices httpDataServices,IStatusServices statusServices)
        {
            _mainAccountStore = accountStore;

            _httpDataServices = httpDataServices;

            _statusServices = statusServices;
        }
        public async Task<int> Login(bool isSeverAuthorise, Account newAccount, string severIp = null)
        {

            if (isSeverAuthorise)
            {
                var Response = await _httpDataServices.GetAuthorizeToken(newAccount);

                if (Response == "invalid")
                {
                    return 0;
                }

                newAccount.Token = Response;

                newAccount.IsAuthorise = true;


            }
            else
            {
                newAccount.IsAuthorise = false;

                newAccount.Password = "";

            }

            _mainAccountStore.CurrentAccount = newAccount;

            return 1;

        }
    }
}
