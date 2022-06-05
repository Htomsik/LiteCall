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


       
        public AccountStore(INavigationService _AuthPageNavigationService)
        {
            this.AuthPageNavigationService = _AuthPageNavigationService;
        }

        INavigationService AuthPageNavigationService;


        private Account _CurrentAccount;

        public Account CurrentAccount
        {
            get => _CurrentAccount;
            set => Set(ref _CurrentAccount, value);
        }


        public void Logout()
        {
            _CurrentAccount=null;

            AuthPageNavigationService.Navigate();
        }
    }
}
