using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;

namespace LiteCall.Infrastructure.Commands
{
    internal class RegistrationCommand : BaseCommand
    {
        private readonly RegistrationPageVMD _RegVMD;
        private readonly INavigatonService<MainPageVMD> _NavigationServices;
        private readonly AccountStore _AccountStore;
        private readonly Func<object, bool> _CanExecute;

        public RegistrationCommand(RegistrationPageVMD RegVMD, INavigatonService<MainPageVMD> navigationServices,
            AccountStore accountStore, Func<object, bool> canExecute = null)
        {
            _RegVMD = RegVMD;
            _NavigationServices = navigationServices;
            _AccountStore = accountStore;
            _CanExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _CanExecute?.Invoke(parameter) ?? true;
        }

        public override void Execute(object parameter)
        {


            if (!CanExecute(parameter)) return;


            Account newAccount = new Account()
            {
                Login = _RegVMD.Login,
                Password = _RegVMD.Password,
            };



            var Response = DataBaseService.Registration(newAccount).Result;


            if(  !string.IsNullOrEmpty(newAccount.Token) || DataBaseService.IsAuthorize(Response) )
            {
                
                newAccount.Token = Response;
                newAccount.IsAuthorise = true;
                _NavigationServices.Navigate();
                
             
            }
            else
            {
                MessageBox.Show("\r\ncould`t get response from server, please check login or password or continue without an account", "Сообщение", MessageBoxButtons.OK);

            }


            _AccountStore.CurrentAccount = newAccount;




        }

    }
}
