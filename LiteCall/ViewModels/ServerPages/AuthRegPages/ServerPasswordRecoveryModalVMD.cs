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
using LiteCall.Stores.ModelStores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerPasswordRecoveryModalVMD:BaseVMD
    {

     

        private readonly IhttpDataServices _httpDataServices;

      

        private readonly CurrentServerStore _currentServerStore;

        public ServerPasswordRecoveryModalVMD(INavigationService authPagenavigationservices, IhttpDataServices httpDataServices, IStatusServices statusServices, CurrentServerStore currentServerStore)
        {
            
            _httpDataServices = httpDataServices;

            _currentServerStore = currentServerStore;

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
                    new ObservableCollection<Question>(await _httpDataServices.GetPasswordRecoveryQestions(_currentServerStore.CurrentServer.ApiIp));

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

            var recoveryModel = new RecoveryModel
            {
                QestionAnswer = QuestionAnswer,
                Question = SelectedQestion,
                recoveryAccount = new Reg_Rec_PasswordAccount { Login = Login, Password = Password }
            };

            await _httpDataServices.PasswordRecovery(recoveryModel, _currentServerStore.CurrentServer.ApiIp);
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
