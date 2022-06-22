using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages
{
    internal class PasswordRecoveryVmd:BaseVmd
    {
        private readonly IGetPassRecoveryQuestionsServices _getPassRecoveryQuestionsServices;

        private readonly IRecoveryPasswordServices _recoveryPasswordServices;

        private readonly IEncryptServices _encryptServices;

        public PasswordRecoveryVmd(INavigationService authPageNavigationServices,IStatusServices statusServices,IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices,IRecoveryPasswordServices recoveryPasswordServices,IEncryptServices encryptServices)
        {
            _getPassRecoveryQuestionsServices = getPassRecoveryQuestionsServices;

            _recoveryPasswordServices = recoveryPasswordServices;

            _encryptServices = encryptServices;


            #region Команды

            #region Асинхронные

            RecoveryPasswordCommand = new AsyncLambdaCommand(OnRecoveryPasswordCommandExecuted,
                (ex) => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
                CanRecoveryPasswordCommandExecute);

            #endregion

            #region Навигационные

            OpenAuthPageCommand = new NavigationCommand(authPageNavigationServices);

            #endregion

            #endregion

            GetQuestionList();
        }

        #region Методы

        private async void GetQuestionList()
        {
            try
            {
                QuestionsCollection =
                    new ObservableCollection<Question>((await _getPassRecoveryQuestionsServices.GetQuestions())!);

                CanServerConnect = true;
            }
            catch 
            {
                CanServerConnect = false;

                OpenAuthPageCommand.Execute(null);
            }

        }

        #endregion

        #region Команды

        public ICommand OpenAuthPageCommand { get; set; }
        public ICommand RecoveryPasswordCommand { get; }

        private bool CanRecoveryPasswordCommandExecute(object p)
        {

            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(QuestionAnswer) && SelectedQuestion is not null;
        }

        private async Task OnRecoveryPasswordCommandExecuted(object p)
        {
            var base64Sha1Password = await _encryptServices.Sha1Encrypt(Password);

            base64Sha1Password = await _encryptServices.Base64Encrypt(base64Sha1Password);

            var recoveryModel = new RecoveryModel
            {
                QestionAnswer = QuestionAnswer,
                Question = SelectedQuestion,
                RecoveryAccount = new RegRecPasswordAccount { Login = Login,Password = base64Sha1Password}
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


        private string? _login;
        public string? Login
        {
            get => _login;
            set => Set(ref _login, value);
        }


        private string? _password;
        public string? Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string? _questionAnswer;

        public string? QuestionAnswer
        {
            get => _questionAnswer;
            set => Set(ref _questionAnswer, value);
        }


        private ObservableCollection<Question>? _questionsCollection;

        public ObservableCollection<Question>? QuestionsCollection
        {
            get => _questionsCollection;
            set => Set(ref _questionsCollection, value);
        }


        private Question? _selectedQuestion;

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => Set(ref _selectedQuestion, value);
        }

        #endregion

    }
}
