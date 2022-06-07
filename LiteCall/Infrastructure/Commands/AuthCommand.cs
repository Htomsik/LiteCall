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

        private readonly AccountStore _AccountStore;

        private readonly Func<object, bool> _CanExecute;

       
        public AuthCommand(AuthorisationPageVMD AuthVMD,
            AccountStore accountStore, Action<Exception> onException, Func<object, bool> canExecute = null) : base(onException)
        {
            _AuthVMD = AuthVMD;
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


            Account newAccount = new Account()
            {
                Login = _AuthVMD.Login,
                Password = _AuthVMD.Password,
            };

            ILoginServices loginServices = new LoginSevices<AccountStore>(_AccountStore);

            _AuthVMD.StatusMessage = "Connecting to server. . .";

          var LoginStatus = await loginServices.Login(!_AuthVMD.CheckStatus,newAccount,"localhost:5005");

          if (LoginStatus)
          {
              _AuthVMD.StatusMessage = "Loggin sucsesfull. . .";
          }

          _AuthVMD.StatusMessage = string.Empty;

        }

        
    }
}
