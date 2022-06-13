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

        private readonly AccountStore _MainAccountStore;


        public AuthoisationMainServerServices(AccountStore accountStore)
        {
            _MainAccountStore = accountStore;
        }
        public async Task<int> Login(bool isSeverAuthorise, Account _NewAccount, string SeverIp = null)
        {

            if (isSeverAuthorise)
            {
                var Response = await DataBaseService.GetAuthorizeToken(_NewAccount);

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

                if(Response == "invalid")
                {
                    MessageBox.Show("The server is not responding. The account will be local", "Сообщение");
                }
                else
                {
                    _NewAccount.Token= Response;
                }

            }

            _MainAccountStore.CurrentAccount = _NewAccount;

            return 1;

        }
    }
}
