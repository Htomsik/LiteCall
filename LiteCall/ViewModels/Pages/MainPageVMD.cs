using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.ServerPages;
using LiteCall.Views.Pages;
using SignalRServ;

namespace LiteCall.ViewModels.Pages
{
    internal class MainPageVMD:BaseVMD
    {
        public MainPageVMD(AccountStore accountStore,ServerAccountStore serverAccountStore, 
                ServersAccountsStore serversAccountsStore,
                CurrentServerStore currentServerStore, 
                MainPageServerNavigationStore mainPageServerNavigationStore, 
                INavigationService settingsPageNavigationService, 
                INavigationService serverPageNavigationService,
                INavigationService openModalServerAuthorisationNavigationService,
                IAuthorisationServices authorisationApiServices)
        {
            AccountStore = accountStore;

            ServerAccountStore = serverAccountStore;

            ServersAccountsStore = serversAccountsStore;
            
            CurrentServerStore = currentServerStore;


            AuthorisationServices = authorisationApiServices;

            MainPageServerNavigationStore = mainPageServerNavigationStore;

            ServerPageNavigationService = serverPageNavigationService;




            ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorisationNavigationService, CanModalRegistrationOpenCommandExecuted);



            VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);

            OpenModalCommaCommand=new LambdaCommand(OnOpenModalCommaExecuted);

          

            DisconnectServerCommand = new LambdaCommand(OnDisconnectServerExecuted,CanDisconnectServerExecute);

            AccountLogoutCommand = new LambdaCommand(OnAccountLogoutExecuted); //Не работает

            OpenSettingsCommand = new NavigationCommand(settingsPageNavigationService);




            SaveServerCommand = new AsyncLamdaCommand(OnSaveServerCommandExecuted, (ex) => StatusMessage = ex.Message, CanSaveServerCommandExecute);

            DeleteServerSavedCommand = new AsyncLamdaCommand(OnDeleteServerSavedExecuted, (ex) => StatusMessage = ex.Message,CanDeleteServerSavedExecute);



            ConnectServerCommand = new AsyncLamdaCommand(OnConnectServerExecuted, (ex) => StatusMessage = ex.Message);

            ConnectServerSavedCommand = new AsyncLamdaCommand(OnConnectServerSavedExecuted, (ex) => StatusMessage = ex.Message, CanConnectServerSavedExecute);




            DisconectSeverReloader.Reloader += DisconectServer;

            MainPageServerNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }




        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(selectedViewModel));
        }

        #region Команды


        public ICommand OpenSettingsCommand { get; }

        public ICommand ModalRegistrationOpenCommand { get; set; }

        private bool CanModalRegistrationOpenCommandExecuted()
        {
            if (ServerAccountStore.CurrentAccount != default)
            {
                return !ServerAccountStore.CurrentAccount.IsAuthorise;
            }

            return false;

        }
           



        public ICommand ConnectServerSavedCommand { get; }

        private bool CanConnectServerSavedExecute(object p)
        {

            if (CurrentServerStore.CurrentServer != default && SelectedServerAccount is not null)
            {
                return SelectedServerAccount.SavedServer.ApiIp != CurrentServerStore.CurrentServer.ApiIp;
            }
            return true;

        }

        private async  Task OnConnectServerSavedExecuted(object p)
        {


            var ServerStatus = await Task.Run(() => CheckServerStatus(SelectedServerAccount.SavedServer.ApiIp));

            if (!ServerStatus)
            {
                return;
            }

            if (selectedViewModel != default)
            {
                DisconectServer();
            }


            Account newServerAccount = new Account
            {
                Login = SelectedServerAccount.Account.Login
            };


            try
            {
                var authoriseStatus = await AuthorisationServices.Login(SelectedServerAccount.Account.IsAuthorise, SelectedServerAccount.Account, SelectedServerAccount.SavedServer.ApiIp);

                if (authoriseStatus == 0)
                {
                    MessageBox.Show("Authorization error. You will be logged without account", "Сообщение");

                    await AuthorisationServices.Login(false, newServerAccount, SelectedServerAccount.SavedServer.ApiIp);
                }
                else
                {
                    newServerAccount = SelectedServerAccount.Account;
                }
            }
            catch (Exception e)
            {

                await AuthorisationServices.Login(false, newServerAccount, SelectedServerAccount.SavedServer.ApiIp);

            }


             ServerStatus = await Task.Run(() => CheckServerStatus(SelectedServerAccount.SavedServer.Ip));

            if (ServerStatus)
            {
                CurrentServerStore.CurrentServer = SelectedServerAccount.SavedServer;

                ServerPageNavigationService.Navigate();

                ServernameOrIp = String.Empty;

                VisibilitiStatus = Visibility.Visible;
            }


        }




        public ICommand SaveServerCommand { get; set; }

        private bool CanSaveServerCommandExecute(object p)
        {


            if (CurrentServerStore.CurrentServer == null)
            {
                return false;
            }

            try
            {
                var IsCurrentServerSaved = ServersAccountsStore.SavedServerAccounts.FirstOrDefault(x => x.SavedServer.ApiIp == CurrentServerStore.CurrentServer.ApiIp);

                return IsCurrentServerSaved is null;
            }
            catch (Exception e)
            {
                return false;
            }
          
        }
        private async Task OnSaveServerCommandExecuted(object p)
        {
            ServersAccountsStore.add(new ServerAccount{Account = ServerAccountStore.CurrentAccount,SavedServer = CurrentServerStore.CurrentServer});
        }




        public ICommand DeleteServerSavedCommand { get; }

        private bool CanDeleteServerSavedExecute(object p) => SelectedServerAccount is not null;

        private async Task OnDeleteServerSavedExecuted(object p)
        {
            ServersAccountsStore.remove(SelectedServerAccount);
        }



        public ICommand DisconnectServerCommand { get; }

        private bool CanDisconnectServerExecute(object p) => true;

        private void OnDisconnectServerExecuted(object p)
        {
            DisconectServer();
        }


        public ICommand VisibilitySwitchCommand { get; }
        private void OnVisibilitySwitchExecuted(object p)
        {
            if (Convert.ToInt32(p) == 1)
            {
                VisibilitiStatus = Visibility.Collapsed;
            }
            else
            {
                VisibilitiStatus = Visibility.Visible;
            }
               
        }


        public ICommand OpenModalCommaCommand { get; }
        private void OnOpenModalCommaExecuted(object p)
        {
            if ((string)p == "1")
            {
                ModalStatus = true;
            }
            else
            {
                ModalStatus = false;
                ServernameOrIp = String.Empty;

            }

        }


        public ICommand AccountLogoutCommand { get; }

        private void OnAccountLogoutExecuted(object p)
        {
            if (selectedViewModel!= null)
            {
                //selectedViewModel.Dispose();
               MainPageServerNavigationStore.Close();
            }

            CurrentServerStore.CurrentServer = default;

            VisibilitiStatus = Visibility.Collapsed;
            this.AccountStore.Logout();
        }

      


        public ICommand ConnectServerCommand { get; }

        private async Task OnConnectServerExecuted(object p)
        {

            Account ServerAccount = new Account
            {
                Login = AccountStore.CurrentAccount.Login
            };

            Server newServer = new Server();

            string ApiIp;

            StatusMessage = "Get server ip. . .";

            if (!CheckStatus)
            {
                //Получить информацию о сервере из главной базы по имени
                ApiIp = await DataBaseService.MainServerGetApiIPI(ServernameOrIp);

                if (ApiIp == null)
                {

                    StatusMessage = string.Empty;
                   
                    return;

                }

                newServer = await DataBaseService.ApiServerGetInfo(ApiIp);

                if (newServer == null)
                {
                    StatusMessage = string.Empty;

                    return;
                }

                newServer.ApiIp = ApiIp;
            }
            else
            {
                
                // Получить информацию из API сервера о сервере
                newServer = await DataBaseService.ApiServerGetInfo(ServernameOrIp);

                if (newServer == null)
                {

                    StatusMessage = string.Empty;


                    return;

                }

                newServer.ApiIp = ServernameOrIp;
            }


            StatusMessage = "Login into server account. . .";

            try
            { 
                var DictionaryServerAccount = ServersAccountsStore.SavedServerAccounts.First(s => s.SavedServer.ApiIp == newServer.ApiIp.ToLower());

              var AuthoriseStatus =  await AuthorisationServices.Login(DictionaryServerAccount.Account.IsAuthorise, DictionaryServerAccount.Account, newServer.ApiIp);

              if (AuthoriseStatus == 0)
              {
                  MessageBox.Show("Authorization error. You will be logged without account", "Сообщение");

                  await AuthorisationServices.Login(false, ServerAccount, newServer.ApiIp);
              }
              else
              {
                  ServerAccount = DictionaryServerAccount.Account;
              }
            }
            catch (Exception e)
            {

                await AuthorisationServices.Login(false, ServerAccount, newServer.ApiIp);

            }

            StatusMessage = "Check sever status. . .";

            //Временно
            newServer.Ip = newServer.Ip.Replace("https://", "");

            bool ServerStatus = await Task.Run(() => CheckServerStatus(newServer.Ip));

            if (newServer is not null && ServerStatus)
            {
                
                CurrentServerStore.CurrentServer = newServer;

               StatusMessage = "Sever status sucsesfull. . .";

                await Task.Delay(250);

                StatusMessage = "Сonnect to server. . .";

               await Task.Delay(250);

               ModalStatus = false;

               ServerPageNavigationService.Navigate();

                 ServernameOrIp = String.Empty;

               VisibilitiStatus=Visibility.Visible;
            }
            
            StatusMessage = string.Empty;

        }


        #endregion

        #region Данные с


        private bool _CheckStatus;
        public bool CheckStatus
        {
            get => _CheckStatus;
            set => Set(ref _CheckStatus, value);
        }

        
        private bool _ModalStatus;

        public bool ModalStatus
        {
            get => _ModalStatus;
            set => Set(ref _ModalStatus, value);
        }



        private AccountStore _AccountStore;

        public AccountStore AccountStore
        {
            get => _AccountStore;
            set => Set(ref _AccountStore, value);
        }

        private CurrentServerStore _CurrentServerStore;

        public CurrentServerStore CurrentServerStore
        {
            get => _CurrentServerStore;
            set => Set(ref _CurrentServerStore, value);
        }


        private ServerAccountStore _ServerAccountStore;
        public ServerAccountStore ServerAccountStore
        {
            get => _ServerAccountStore;
            set => Set(ref _ServerAccountStore, value);
        }


        private ServerAccount _selectedServerAccount;

        public ServerAccount SelectedServerAccount
        {
            get => _selectedServerAccount;
            set => Set(ref _selectedServerAccount, value);
        }


        private ServersAccountsStore _ServersAccountsStore;
        public ServersAccountsStore ServersAccountsStore
        {
            get => _ServersAccountsStore;
            set => Set(ref _ServersAccountsStore, value);
        }


        private IAuthorisationServices _AuthorisationServices;

        public IAuthorisationServices AuthorisationServices
        {
            get => _AuthorisationServices;
            set => Set(ref _AuthorisationServices, value);
        }




        private string _servernameOrIp;

        public string ServernameOrIp
        {
            get => _servernameOrIp;
            set => Set(ref _servernameOrIp, value);
        }



        private Visibility _VisibilitiStatus = Visibility.Collapsed;
        public Visibility VisibilitiStatus
        {
            get => _VisibilitiStatus;
            set => Set(ref _VisibilitiStatus, value);
        }


        private readonly INavigationService ServerPageNavigationService;

        public MainPageServerNavigationStore MainPageServerNavigationStore;

        public BaseVMD selectedViewModel =>  MainPageServerNavigationStore.MainPageServerCurrentViewModel;



        //Сообщение об ошибке
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


        //Есть ли сообщение об ошибке
        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);




        #endregion


       bool  CheckServerStatus(string serverAdress)
        {

            string[] ServerAddresArray = serverAdress.Split(':');

            if (ServerAddresArray.Length ==2)
            {
                try
                {
                    using (var client =  new TcpClient(ServerAddresArray[0], Convert.ToInt32(ServerAddresArray[1])))
                        return true;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message, "Сообщение");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Incorrect Ip adrress", "Сообщение");
                return false;

            }


        }

       private void DisconectServer()
       {

           if (selectedViewModel == null) return;

           selectedViewModel.Dispose();

           VisibilitiStatus = Visibility.Collapsed;

           MainPageServerNavigationStore.MainPageServerCurrentViewModel = null;
       }



    }
}
