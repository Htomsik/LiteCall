using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;
using Microsoft.AspNetCore.WebUtilities;


namespace LiteCall.Infrastructure.Commands
{
    internal class AuthCommand : AsyncCommandBase
    {

        private readonly AuthorisationPageVMD _AuthVMD;
        private readonly INavigatonService<MainPageVMD> _NavigationServices;
        private readonly AccountStore _AccountStore;
        private readonly Func<object, bool> _CanExecute;
        public AuthCommand(AuthorisationPageVMD AuthVMD, INavigatonService<MainPageVMD> navigationServices,
            AccountStore accountStore, Action<Exception> onException, Func<object, bool> canExecute = null) : base(onException)
        {
            _AuthVMD = AuthVMD;
            _NavigationServices = navigationServices;
            _AccountStore = accountStore;
            _CanExecute = canExecute;
        }
        public override bool CanExecute(object parameter)
        {
            if (!IsExecuting)
            {
                return (_CanExecute?.Invoke(parameter) ?? true);
            }
            else
            {
                return false;
            }

        }

        protected  override async Task ExecuteAsync(object parameter)
        {

            _AuthVMD.StatusMessage = "Connecting to server. . .";

            Account newAccount = new Account()
            {
                Login = _AuthVMD.Login,
                Password = _AuthVMD.Password,
            };

            //Если авторизирован

            if (!_AuthVMD.CheckStatus)
            {
                
                var Response = await DataBaseService.GetAuthorizeToken(newAccount);

                //Если появился msbox то откат всего
                if (Response == "invalid")
                {
                    _AuthVMD.StatusMessage = string.Empty;
                    return;
                }
                newAccount.Token = Response;
                newAccount.IsAuthorise = true;


                //Задержка перед открытием
                _AuthVMD.StatusMessage = "Loggin sucsesfull. . .";
                 await  Task.Delay(1000);


                _NavigationServices.Navigate();
            }
            else
            {
                newAccount.IsAuthorise = false;
                newAccount.Password = "";
                newAccount.Token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJOYW1lIjoiQW5vbnltb3VzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQW5vbnltb3VzIiwiaXNzIjoiTGl0ZUNhbGwiLCJhdWQiOiJDbGllbnRMaXRlQ2FsbCJ9.qk3A9sj0K7ColqaEmKV7Mk0ux-up9KxUE_V_o0GoM5Z93QJgB16kx_5DlyDbEhjzjyZzW1MikQCoUDMhwv6Q9WWFR_U9uUPV1XPYusGUy7bPyRT_x3AWBoxYzVFllUpmiQyUvUlyUflgNBSzyxugfN9X-0EBZ32PaTfmBMzugtZrvFTyXvJCzt3Rn-OSEc9JWlkfazYVWHEs5gxdpa8NqE4lQZK5iSWTU8GgfbHaji1N7tE87YoZgjUNHWxUo-7fnrR-aRvVBu06t8cdZdsKvvuXuOUUS0hqwwDSENDSGR0tPLWuWHZ3jWr-qelTss2G9fGOhJVXmgPvqf6-9VE9PA";


                //Задержка перед открытием
                _AuthVMD.StatusMessage = "Loggin sucsesfull. . .";
                await Task.Delay(1000);

                _NavigationServices.Navigate();

            }
            _AccountStore.CurrentAccount = newAccount;
            _AuthVMD.StatusMessage = string.Empty;


        }

        
    }
}
