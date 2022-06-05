using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;

namespace LiteCall.Stores
{
    internal class AccountStore:BaseVMD
    {
        private static Account DefaultAccount = new Account { Login = "LC_User" };


        public AccountStore(INavigationService _AuthPageNavigationService)
        {
            this.AuthPageNavigationService = _AuthPageNavigationService;
        }

        INavigationService AuthPageNavigationService;


        private Account _CurrentAccount = DefaultAccount;

        public Account CurrentAccount
        {
            get => _CurrentAccount;
            set => Set(ref _CurrentAccount, value);
        }


        public void Logout()
        {
            _CurrentAccount = DefaultAccount;

            AuthPageNavigationService.Navigate();
        }
    }
}
