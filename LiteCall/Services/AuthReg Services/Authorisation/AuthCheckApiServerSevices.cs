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
        private readonly ServerAccountStore ServerAccountStore;

        private readonly IhttpDataServices HttpDataServices;

        public AuthCheckApiServerSevices(ServerAccountStore serverAccountStore, IhttpDataServices httpDataServices)
        {
            ServerAccountStore = serverAccountStore;

            HttpDataServices = httpDataServices;
        }

        public async Task<int> Login(bool IsAuthTokenAuthorise, Account _NewAccount, string ApiServerIp)
        {
            if (IsAuthTokenAuthorise)
            {
                var Response = await HttpDataServices.GetAuthorizeToken(_NewAccount, ApiServerIp);

                if (Response == "invalid")
                {
                    return 0;
                }
                _NewAccount.Role = await HttpDataServices.GetRoleFromJwtToken(Response);

                _NewAccount.Token = Response;

                _NewAccount.IsAuthorise = true;
            }
            else
            {
                _NewAccount.IsAuthorise = false;

                _NewAccount.Password = "";

                var Response= await HttpDataServices.GetAuthorizeToken(_NewAccount, ApiServerIp);


                if (Response == "invalid")
                {
                    return 0;
                }

                _NewAccount.Role = await HttpDataServices.GetRoleFromJwtToken(Response);

                _NewAccount.Token = Response;

            }

            ServerAccountStore.CurrentAccount = _NewAccount;

            return 1;
        }
    }
}
