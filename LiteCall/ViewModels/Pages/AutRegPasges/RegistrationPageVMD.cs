using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public RegistrationPageVMD(INavigationService authPagenavigationservices, IRegistrationSevices registrationSevices,IStatusServices statusServices, ICaptchaServices captchaServices, IGetPasswordRecoveryQuestions getPasswordRecoveryQuestions)
        {
            
            _registrationSevices = registrationSevices;

            
            _captchaServices = captchaServices;

            _getPasswordRecoveryQuestions = getPasswordRecoveryQuestions;


            RegistrationCommand = new AsyncLamdaCommand(OnRegistrationExecuted, (ex) => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
                CanRegistrationExecute);

            OpenAuthPageCommand = new NavigationCommand(authPagenavigationservices);

            OpenModalCommand = new AsyncLamdaCommand(OnOpenModalCommamdExecuted, (ex) => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanOpenModalCommamdExecute);


            GetQuestionList();
        }


        private async void GetQuestionList()
        {
            try
            {
                QestionsCollection =
                    new ObservableCollection<Question>(await _getPasswordRecoveryQuestions.GetQestions());

                CanServerConnect = true;
            }
            catch (Exception e)
            {
                CanServerConnect = false;
            }
            
        }

        #region Commands


        public ICommand RegistrationCommand { get; }
        private bool CanRegistrationExecute(object p)=> !(bool)p && !string.IsNullOrEmpty(CapthcaString);

        private async Task OnRegistrationExecuted(object p)
        {
            var newAccount = new Account()
            {
                Login = this.Login,
                Password = this.Password
            };

            var newRegistrationmodel = new RegistrationModel()
            {
                Captcha = CapthcaString,
                QestionAnswer = QuestionAnswer,
                Question = SelectedQestion,
                recoveryAccount = newAccount
            };

            var response = await _registrationSevices.Registration( newRegistrationmodel);

            switch (response)
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
            
            if (Password != ConfirmPassword) return false;
            
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && !string.IsNullOrEmpty(QuestionAnswer);

        }


        public async Task<bool> GetCaptcha()
        {

            Capthca = await _captchaServices.GetCaptcha();

            if (Capthca == null)
            {
                return false;
            }

            return true;
        }


        public ICommand OpenAuthPageCommand { get; }


        #endregion


        private bool _CanServerConnect;

        public bool CanServerConnect
        {
            get => _CanServerConnect;
            set => Set(ref _CanServerConnect, value);
        }


        private bool _modalStatus;

        public bool ModalStatus
        {
            get => _modalStatus;
            set => Set(ref _modalStatus, value);
        }


        private string _capthcaString;

        public string CapthcaString
        {
            get => _capthcaString;
            set => Set(ref _capthcaString, value);
        }


        private ImageSource _capthca;

        public ImageSource? Capthca
        {
            get => _capthca;
            set => Set(ref _capthca, value);
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

       

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => Set(ref _confirmPassword, value);

        }


        private string _questionAnswer;

        public string QuestionAnswer
        {
            get => _questionAnswer;
            set => Set(ref _questionAnswer, value);
        }


        private ObservableCollection<Question> _qestionsCollection;

        public ObservableCollection<Question> QestionsCollection
        {
            get => _qestionsCollection;
            set => Set(ref _qestionsCollection, value);
        }


        private Question _selectedQestion;

        public Question SelectedQestion
        {
            get => _selectedQestion;
            set => Set(ref _selectedQestion, value);
        }



        private readonly IRegistrationSevices _registrationSevices;

        private readonly ICaptchaServices _captchaServices;

        private readonly IGetPasswordRecoveryQuestions _getPasswordRecoveryQuestions;

     
    }
}
