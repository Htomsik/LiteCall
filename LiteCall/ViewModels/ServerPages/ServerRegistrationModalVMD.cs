﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerRegistrationModalVMD:BaseVMD
    {
        public ServerRegistrationModalVMD(INavigationService closeModalNavigationService, IRegistrationSevices registrationSevices,CurrentServerStore currentServerStore)
        {

            _CurrentServerStore = currentServerStore;

            _RegistrationSevices = registrationSevices;

            RegistrationCommand = new AsyncLamdaCommand(OnRegistrationExecuted, (ex) => StatusMessage = ex.Message,
                CanRegistrationExecute);

            OpenAuthPageCommand = new NavigationCommand(closeModalNavigationService);

            OpenModalCommand = new AsyncLamdaCommand(OnOpenModalCommamdExecuted, (ex) => StatusMessage = ex.Message, CanOpenModalCommamdExecute);

        }





        public ICommand RegistrationCommand { get; }
        private bool CanRegistrationExecute(object p) => !(bool)p && !string.IsNullOrEmpty(CapthcaString);

        private async Task OnRegistrationExecuted(object p)
        {
            var newAccount = new Account
            {
                Login = this.Login,
                Password = this.Password,
            };

            var Response = await _RegistrationSevices.Registration(newAccount, CapthcaString);

            switch (Response)
            {
                case 0:
                    GetCaptcha();
                    break;

                case 1:
                    ModalStatus = false;
                    StatusMessage = null;
                    break;

            }
        }



        public ICommand OpenModalCommand { get; }
        private async Task OnOpenModalCommamdExecuted(object p)
        {

            if (ModalStatus == false)
            {

                StatusMessage = "Connecting to server. . .";

                var Status = await GetCaptcha();

                if (!Status)
                {
                    StatusMessage = string.Empty;
                    return;
                }

                ModalStatus = true;

                StatusMessage = string.Empty;
            }
            else
            {
                ModalStatus = false;

                CapthcaString = string.Empty;

            }

        }


        private bool CanOpenModalCommamdExecute(object p)
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


        public async Task<bool> GetCaptcha()
        {
            ModalStatusMessage = "Get new Captcha. . .";

            var receive_bytes = await DataBaseService.GetCaptcha(_CurrentServerStore.CurrentServer.ApiIp);

            if (receive_bytes != null)
            {

                var CaptchaFromServer = ImageBox.BytesToImage(receive_bytes.GetRawData());

                Capthca = DataBaseService.GetImageStream(CaptchaFromServer);

                ModalStatusMessage = string.Empty;

                return true;
            }

            return false;
        }



        /// <summary>
        /// Переход на окно авторизации
        /// </summary>
        public ICommand OpenAuthPageCommand { get; }


        private bool _ModalStatus;

        public bool ModalStatus
        {
            get => _ModalStatus;
            set => Set(ref _ModalStatus, value);
        }


        private string _CapthcaString;

        public string CapthcaString
        {
            get => _CapthcaString;
            set => Set(ref _CapthcaString, value);
        }


        private ImageSource _Capthca;

        public ImageSource Capthca
        {
            get => _Capthca;
            set => Set(ref _Capthca, value);
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


        private string _ConfirmPassword;
        public string ConfirmPassword
        {
            get => _ConfirmPassword;
            set => Set(ref _ConfirmPassword, value);
        }





        private bool _IsNotApiRegistration = false;
        public bool IsNotApiRegistration
        {
            get => _IsNotApiRegistration;
            set
            {
                Set(ref _IsNotApiRegistration, value);

            }
        }


        private CurrentServerStore _CurrentServerStore;

        private IRegistrationSevices _RegistrationSevices;

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


        //Для модального окна
        private string _ModalstatusMessage;


        public string ModalStatusMessage
        {
            get => _ModalstatusMessage;
            set
            {
                Set(ref _ModalstatusMessage, value);
                OnPropertyChanged(nameof(ModalHasStatusMessage));
            }
        }

        public bool ModalHasStatusMessage => !string.IsNullOrEmpty(ModalStatusMessage);



    }
}