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

        public bool isDefaulAccount => CurrentAccount == DefaultAccount;

        public AccountStore(INavigationService _AuthPageNavigationService)
        {
            this.AuthPageNavigationService = _AuthPageNavigationService;
        }

        INavigationService AuthPageNavigationService;


        public event Action CurrentAccountChange;


        private void OnCurrentAccountChangeChanged()
        {
            CurrentAccountChange?.Invoke();
        }

        private Account _CurrentAccount = DefaultAccount;

        public Account CurrentAccount
        {
            get => _CurrentAccount;
            set
            {
                Set(ref _CurrentAccount, value);
                OnCurrentAccountChangeChanged();
            } 
        }


        public void Logout()
        {
            _CurrentAccount = DefaultAccount;

            AuthPageNavigationService.Navigate();
        }
    }
}
