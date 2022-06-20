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
    internal class AuthorisationPageVMD : BaseVMD
    {
        private readonly IEncryptServices _encryptServices;

        public AuthorisationPageVMD(INavigationService registrationNavigationServices,INavigationService passwordRecoveryNavigationService, IAuthorisationServices authorisationServices,IEncryptServices encryptServices)
        {
            _encryptServices = encryptServices;

            AuthorisationServices = authorisationServices;

            AuthCommand =
                new AsyncLamdaCommand(OnAuthExecuteExecuted, (ex) => StatusMessage = ex.Message, CanAuthExecute);

            OpenRegistrationPageCommand = new NavigationCommand(registrationNavigationServices);

            OpenRecoveryPasswordPageCommand = new NavigationCommand(passwordRecoveryNavigationService);

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

        private async Task OnAuthExecuteExecuted(object p)
        {
            var Base64Sha1Password = await _encryptServices.Sha1Encrypt(Password);

            Base64Sha1Password = await _encryptServices.Base64Encypt(Base64Sha1Password);

            var newAccount = new Account
            {
                Login = this.Login,
                Password = Base64Sha1Password
            };

            var Response = await AuthorisationServices.Login(!CheckStatus, newAccount);

            switch (Response)
            {
                case 0:
                    break;

                case 1:
                    StatusMessage = null;
                    break;

            }
        }

        public ICommand OpenRegistrationPageCommand { get; }

        public ICommand OpenRecoveryPasswordPageCommand { get; }


        #endregion

        #region Данные с формы


        public IAuthorisationServices AuthorisationServices { get; }



        private bool _checkStatus;
        public bool CheckStatus
        {
            get => _checkStatus;
            set => Set(ref _checkStatus, value);
        }


        private string _login;
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }


        private string _password;

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
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
