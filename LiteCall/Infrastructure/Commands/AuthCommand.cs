using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.ViewModels.Pages;

namespace LiteCall.Infrastructure.Commands
{
    internal class AuthCommand : BaseCommand
    {

        private readonly AuthorisationPageVMD _AuthVMD;
        private readonly ParametrNavigationServices<Account,MainPageVMD> _NavigationServices;
        private readonly Func<object, bool> _CanExecute;
        public AuthCommand(AuthorisationPageVMD authVmd, ParametrNavigationServices<Account,MainPageVMD> navigationServices, Func<object, bool> canExecute=null)
        {
            _AuthVMD = authVmd;
            _NavigationServices = navigationServices;
            _CanExecute = canExecute;
        }
        public override bool CanExecute(object parameter)
        {
          return  _CanExecute?.Invoke(parameter) ?? true;
        }

        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;


            Account newAccount = new Account()
            {
               Login = _AuthVMD.Login,
               Password = _AuthVMD.Password,
               Type = _AuthVMD.CheckStatus
            };

            _NavigationServices.Navigate(newAccount);
        }
    }
}
