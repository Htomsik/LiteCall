using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.Authorisation
{
    internal class AuthCheckApiServerSevices:IAuthorisationServices
    {
        private ServerAccountStore ServerAccountStore;

        public AuthCheckApiServerSevices(ServerAccountStore serverAccountStore)
        {
            ServerAccountStore = serverAccountStore;
        }

        public async Task<int> Login(bool IsAuthTokenAuthorise, Account _NewAccount, string ApiServerIp)
        {
            if (IsAuthTokenAuthorise)
            {
                var Response = await DataBaseService.GetAuthorizeToken(_NewAccount, ApiServerIp);

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

                _NewAccount.Token = await DataBaseService.GetAuthorizeToken(_NewAccount, ApiServerIp);

            }

            ServerAccountStore.CurrentAccount = _NewAccount;

            return 1;
        }
    }
}
