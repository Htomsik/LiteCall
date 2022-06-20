using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages
{
    internal class PasswordRecoveryVMD:BaseVMD
    {
        private readonly IGetPassRecoveryQuestionsServices _getPassRecoveryQuestionsServices;

        private readonly IRecoveryPasswordServices _recoveryPasswordServices;

        private readonly IEncryptServices _encryptServices;

        public PasswordRecoveryVMD(INavigationService authPagenavigationservices,IStatusServices statusServices,IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices,IRecoveryPasswordServices recoveryPasswordServices,IEncryptServices encryptServices)
        {
            _getPassRecoveryQuestionsServices = getPassRecoveryQuestionsServices;

            _recoveryPasswordServices = recoveryPasswordServices;

            _encryptServices = encryptServices;


            #region Команды

            #region Асинхронные

            RecoveryPasswordCommand = new AsyncLamdaCommand(OnRecoveryPasswordCommandExecuted,
                (ex) => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
                CanRecoveryPasswordCommandExecute);

            #endregion

            #region Навигационные

            OpenAuthPageCommand = new NavigationCommand(authPagenavigationservices);

            #endregion

            #endregion

            GetQuestionList();
        }

        #region Методы

        private async void GetQuestionList()
        {
            try
            {
                QestionsCollection =
                    new ObservableCollection<Question>(await _getPassRecoveryQuestionsServices.GetQestions());

                CanServerConnect = true;
            }
            catch (Exception e)
            {
                CanServerConnect = false;
            }

        }

        #endregion

        #region Команды

        public ICommand OpenAuthPageCommand { get; set; }
        public ICommand RecoveryPasswordCommand { get; }

        private bool CanRecoveryPasswordCommandExecute(object p)
        {

            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(QuestionAnswer) && SelectedQestion is not null;
        }

        private async Task OnRecoveryPasswordCommandExecuted(object p)
        {
            var Base64Sha1Password = await _encryptServices.Sha1Encrypt(Password);

            Base64Sha1Password = await _encryptServices.Base64Encypt(Base64Sha1Password);

            var recoveryModel = new RecoveryModel
            {
                QestionAnswer = QuestionAnswer,
                Question = SelectedQestion,
                recoveryAccount = new Reg_Rec_PasswordAccount { Login = Login,Password = Base64Sha1Password}
            };


            if (await _recoveryPasswordServices.RecoveryPassword(recoveryModel))
            {
                OpenAuthPageCommand.Execute(null);
            }
           
        }

        #endregion

        #region Данные с окна



        private bool _canServerConnect;

        public bool CanServerConnect
        {
            get => _canServerConnect;
            set => Set(ref _canServerConnect, value);
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

        #endregion

    }
}
