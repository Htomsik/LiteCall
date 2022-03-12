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


        private bool IsAuthorize(string token)
        {

            try
            {
                dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.Split('.')[1])));
                return (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] != "Anonymous" ? true : false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

          
        }

        public override void Execute(object parameter)
        {

        

            Account newAccount = new Account()
            {
                Login = _AuthVMD.Login,
                Password = _AuthVMD.Password,
            };



            if (!_AuthVMD.CheckStatus)
            {
                
                var Response = DataBaseService.GetAuthorizeToken(newAccount).Result;

                if (Response == "No Connect")
                {
                   return;
                }
                else if (Response == "Invalid Data")
                {
                    MessageBox.Show("Invalid login or password", "Сообщение", MessageBoxButtons.OK);
                    return;
                }


                newAccount.Token = Response;

                if (IsAuthorize(Response))
                {
                    newAccount.IsAuthorise = true;
                    _NavigationServices.Navigate();
                }
                else 
                {
                    MessageBox.Show("Invalid login or password", "Сообщение", MessageBoxButtons.OK);
                    return;
                }

            }
            else
            {
                newAccount.IsAuthorise = false;
                newAccount.Password = "";

                var Response = DataBaseService.GetAuthorizeToken(newAccount).Result;

                if (Response == "No Connect")
                {
                    return;
                }

                //Проверка на токен
                if (!IsAuthorize(Response))
                {
                    newAccount.Token = Response;
                    _NavigationServices.Navigate();
                }
                else
                {
                    MessageBox.Show("Unknown Error: 0f2ffeb6", "Сообщение", MessageBoxButtons.OK);
                    return;
                }

            }

            _AccountStore.CurrentAccount = newAccount;

        }
    }
}
