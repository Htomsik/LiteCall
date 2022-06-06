using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;

namespace LiteCall.Services
{
    internal  class LoginSevices<TAccountStore>:ILoginServices where TAccountStore : AccountStore,new()
    {
        private readonly TAccountStore _AuthAccountStore;

        public LoginSevices(TAccountStore AuthAccountStore)
        {
            _AuthAccountStore = AuthAccountStore;
        }

        
        public async Task<bool> Login(bool _isServerAuthoisation, Account _NewAccount, string ApiServerIp)
        {
            if (_isServerAuthoisation)
            {

                var Response = await DataBaseService.GetAuthorizeToken(_NewAccount, ApiServerIp);

                //Если появился msbox то откат всего
                if (Response == "invalid")
                {
                    return false;
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

            //Задержка для правильного отображения статусов
            await Task.Delay(1000);


            _AuthAccountStore.CurrentAccount = _NewAccount;

            return true;
        }

       
    }
}
