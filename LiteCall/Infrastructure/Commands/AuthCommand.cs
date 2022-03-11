using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;
using Microsoft.AspNetCore.WebUtilities;


namespace LiteCall.Infrastructure.Commands
{
    internal class AuthCommand : BaseCommand
    {

        private readonly AuthorisationPageVMD _AuthVMD;
        private readonly INavigatonService<MainPageVMD> _NavigationServices;
        private readonly AccountStore _AccountStore;
        private readonly Func<object, bool> _CanExecute;
        public AuthCommand(AuthorisationPageVMD AuthVMD, INavigatonService<MainPageVMD> navigationServices, AccountStore accountStore, Func<object, bool> canExecute=null)
        {
            _AuthVMD = AuthVMD;
            _NavigationServices = navigationServices;
            _AccountStore = accountStore;
            _CanExecute = canExecute;
        }
        public override bool CanExecute(object parameter)
        {
          return  _CanExecute?.Invoke(parameter) ?? true;
        }


        private  bool IsAuthorize(string token)
        {
            dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.Split('.')[1])));
            return (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role%22"] != "Anonymous"? true:false;
        }

        public override void Execute(object parameter)
        {
            

            if (!CanExecute(parameter)) return;



            Account newAccount = new Account()
            {
                Login = _AuthVMD.Login,
                Password = _AuthVMD.Password,
            };



            if (!_AuthVMD.CheckStatus)
            {
                
                try
                {
                    newAccount.Token = DataBaseService.GetAuthorizeToken(newAccount).Result;

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                if (string.IsNullOrEmpty(newAccount.Token))
                {
                    MessageBox.Show("\r\ncould`t get response from server, please check login or password or continue without an account ", "Сообщение", MessageBoxButtons.OK);
                }
                else if (IsAuthorize(newAccount.Token))
                {
                    newAccount.IsAuthorise = true;
                    _NavigationServices.Navigate();
                }
                else
                {
                    MessageBox.Show("This account doesn't exist", "Сообщение", MessageBoxButtons.OK);
                }

               
            }
            else
            {
                newAccount.IsAuthorise = false;
                newAccount.Password = "";

                try
                {
                    newAccount.Token = DataBaseService.GetAuthorizeToken(newAccount).Result;

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                if (string.IsNullOrEmpty(newAccount.Token))
                {
                    MessageBox.Show("\r\ncould`t get response from server, please check login or password or continue without an account ", "Сообщение", MessageBoxButtons.OK);
                }
                else
                {
                    _NavigationServices.Navigate();
                }



            }



            _AccountStore.CurrentAccount = newAccount;

        }
    }
}
