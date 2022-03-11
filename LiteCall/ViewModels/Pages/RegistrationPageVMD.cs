using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages
{
    internal class RegistrationPageVMD:BaseVMD
    {
        public RegistrationPageVMD(AccountStore AccountStore, INavigatonService<MainPageVMD> MainPageNavigationServices,INavigatonService<AuthorisationPageVMD> AuthPagenavigationservices)
        {
            RegistrationCommand = new RegistrationCommand(this, MainPageNavigationServices, AccountStore, CanRegistrationExecute);

            OpenAuthPageCommand = new NavigationCommand<AuthorisationPageVMD>(AuthPagenavigationservices);
        }


        #region Commands

        /// <summary>
        /// Регистрация
        /// </summary>
        public ICommand RegistrationCommand { get; }
        private bool CanRegistrationExecute(object p)
        {


            var param = (Tuple<object, object>)p;

            var logintb = !Convert.ToBoolean(param?.Item1);
            var passtb = !Convert.ToBoolean(param?.Item2);



            if (Password != ConfirmPassword)
            {
                return false;
            }


            return logintb && passtb && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword);

        }


        /// <summary>
        /// Переход на окно авторизации
        /// </summary>
        public ICommand OpenAuthPageCommand { get; }

       
        #endregion








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


        private string _ConfirmPassword;
        public string ConfirmPassword
        {
            get => _ConfirmPassword;
            set => Set(ref _ConfirmPassword, value);
        }



      
    }
}
