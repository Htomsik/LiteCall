using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages
{
    internal class AuthorisationPageVMD:BaseVMD
    {
        public AuthorisationPageVMD(AccountStore AccountStore, INavigatonService<MainPageVMD> MainPageNavigationServices)
        {
            
            // AuthCommand = new NavigationCommand<MainPageVMD>(navigationStore, () => new MainPageVMD(navigationStore));
            //  AuthoCommand = new AuthCommand(new NavigationServices<MainPageVMD>(navigationStore, () => new MainPageVMD(navigationStore)));
            // AuthCommand = new AuthCommand(this, new ParametrNavigationServices<Account, MainPageVMD>(
            //   navigationStore, (account) => new MainPageVMD(navigationStore, account)),CanAuthExecute);

             AuthCommand = new AuthCommand(this, MainPageNavigationServices, AccountStore,CanAuthExecute);



        }
        private readonly NavigationStore _NavigationStore;

        #region Команды


        public ICommand AuthCommand { get; }

        private bool CanAuthExecute(object p)
        {
            if (CheckStatus == true && !string.IsNullOrEmpty(Login))
            {
                return true;
            }

            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);


        }

        #endregion

        #region Данные с формы


        private bool _CheckStatus;
        public bool CheckStatus
        {
            get => _CheckStatus;
            set => Set(ref _CheckStatus, value);
        }


        private string _Login;
        public string Login
        {
            get => _Login;
            set => Set(ref _Login, value);
        }


        private string _Password;

        public string Password
        {
            get => _Password;
            set => Set(ref _Password, value);
        }




        #endregion
    }
}
