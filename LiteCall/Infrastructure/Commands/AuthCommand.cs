﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;


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

        public override void Execute(object parameter)
        {
            

            if (!CanExecute(parameter)) return;


            Account newAccount = new Account()
            {
               Login = _AuthVMD.Login,
               Password = _AuthVMD.Password,
               IsAuthorise = !_AuthVMD.CheckStatus,
            };

            if (newAccount.IsAuthorise)
            {
                newAccount.Token = DataBaseService.GetAuthorizeToken(newAccount).Result;

                if (!string.IsNullOrEmpty(newAccount.Token))
                {
                    _NavigationServices.Navigate();
                }
                else
                {
                    var result = MessageBox.Show("\r\ncould`t get response from server, please check login or password or continue without an account ", "Сообщение", MessageBoxButtons.OK);

                }
            }
            else
            {
                _NavigationServices.Navigate();
            }
            

            _AccountStore.CurrentAccount = newAccount;

            

            
        }
    }
}
