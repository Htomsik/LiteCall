﻿using System;
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

        public AuthorisationPageVMD(AccountStore AccountStore, INavigationService RegistrationPageNavigationServices)
        {

            AuthCommand = new AuthCommand(this, AccountStore, (ex) => StatusMessage = ex.Message, CanAuthExecute);

            OpenRegistrationPageCommand = new NavigationCommand(RegistrationPageNavigationServices);

        }

        #region Команды

        public ICommand AuthCommand { get; }

        private bool CanAuthExecute(object p)
        {

            var param = (Tuple<object, object>)p;
            var logintbValidator = !Convert.ToBoolean(param.Item1);
            var passtbValidator = !Convert.ToBoolean(param.Item2);

            if (CheckStatus && logintbValidator && !string.IsNullOrEmpty(Login))
            {
                return true;
            }
            return logintbValidator && passtbValidator && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);


        }
        /// <summary>
        /// Открыть окно регистрации
        /// </summary>
        public ICommand OpenRegistrationPageCommand { get; }
       

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


      
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                Set(ref _statusMessage, value);
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);


        #endregion
    }
}
