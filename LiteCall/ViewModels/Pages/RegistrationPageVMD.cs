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
        public RegistrationPageVMD(AccountStore AccountStore, INavigatonService<MainPageVMD> MainPageNavigationServices,INavigatonService<AuthorisationPageVMD> AuthPagenavigationservices)
        {
            RegistrationCommand = new RegistrationCommand(CapthcaString, this, MainPageNavigationServices, AccountStore, CanRegistrationExecute);

            OpenAuthPageCommand = new NavigationCommand<AuthorisationPageVMD>(AuthPagenavigationservices);

            OpenModalCommand = new LambdaCommand(OnOpenModalCommamdExecuted,CanOpenModalCommamdExecute);
        }


        #region Commands

        /// <summary>
        /// Регистрация
        /// </summary>
        public ICommand RegistrationCommand { get; }
        private bool CanRegistrationExecute(object p)=> !(bool)p && !string.IsNullOrEmpty(CapthcaString);



        public ICommand OpenModalCommand { get; }
        private void OnOpenModalCommamdExecuted(object p)
        {

            if (ModalStatus == false)
            {

               GetCaptcha();

                ErrorHeight = 0;

                ModalStatus = true;
            }
            else
            {
                ModalStatus = false;

                CapthcaString = string.Empty;

            }

        }



        public void GetCaptcha()
        {
            byte[] receive_bytes = DataBaseService.GetCaptcha().Result.GetRawData();

            var CaptchaFromServer = ImageBox.BytesToImage(receive_bytes);

            Capthca = DataBaseService.GetImageStream(CaptchaFromServer);
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





        /// <summary>
        /// Переход на окно авторизации
        /// </summary>
        public ICommand OpenAuthPageCommand { get; }


        #endregion



        private bool _ModalStatus;

        public bool ModalStatus
        {
            get => _ModalStatus;
            set => Set(ref _ModalStatus, value);
        }



        private double _ErrorHeight = 0;
        public double ErrorHeight
        {
            get => _ErrorHeight;
            set => Set(ref _ErrorHeight, value);
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



      
    }
}
