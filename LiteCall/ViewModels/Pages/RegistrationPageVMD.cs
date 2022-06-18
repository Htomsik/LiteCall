using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages
{
    internal class RegistrationPageVMD:BaseVMD
    {
        public RegistrationPageVMD(AccountStore accountStore,INavigationService authPagenavigationservices, IRegistrationSevices registrationSevices, IhttpDataServices httpDataServices, IimageServices imageServices, IStatusServices statusServices)
        {

            _registrationSevices = registrationSevices;

            _httpDataServices = httpDataServices;

            _imageServices = imageServices;

            RegistrationCommand = new AsyncLamdaCommand(OnRegistrationExecuted, (ex) => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
                CanRegistrationExecute);

            OpenAuthPageCommand = new NavigationCommand(authPagenavigationservices);

            OpenModalCommand = new AsyncLamdaCommand(OnOpenModalCommamdExecuted, (ex) => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanOpenModalCommamdExecute);
        }


        #region Commands

     
        public ICommand RegistrationCommand { get; }
        private bool CanRegistrationExecute(object p)=> !(bool)p && !string.IsNullOrEmpty(CapthcaString);

        private async Task OnRegistrationExecuted(object p)
        {
            var newAccount = new Account
            {
                Login = this.Login,
                Password = this.Password,
            };

            var Response = await _registrationSevices.Registration(newAccount, CapthcaString);

            switch (Response)
            {
                case 0:
                    GetCaptcha();
                    break;

                case 1:
                    ModalStatus = false;
                 
                    break;

            }
        }



        public ICommand OpenModalCommand { get; }
        private async Task OnOpenModalCommamdExecuted(object p)
        {

            if (ModalStatus == false)
            {

               var Status = await GetCaptcha();

               if (!Status)
               {
                  
                    return;
               }

               ModalStatus = true;
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
           

            var receive_bytes = await _httpDataServices.GetCaptcha();

            if (receive_bytes !=null)
            {
                var CaptchaFromServer = ImageBox.BytesToImage(receive_bytes.GetRawData());

                Capthca = _imageServices.GetImageStream(CaptchaFromServer);

                return true;
            }

            return false;
        }


        public ICommand OpenAuthPageCommand { get; }


        #endregion



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


        private readonly IRegistrationSevices _registrationSevices;

        private readonly IhttpDataServices _httpDataServices;

        private readonly IimageServices _imageServices;


    }
}
