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
        public MainPageVMD(AccountStore AccountStore,ServerAccountStore ServerAccountStore, ServersAccountsStore serversAccountsStore, CurrentServerStore CurrentServerStore, 
                MainPageServerNavigationStore MainPageServerNavigationStore, INavigationService SettingsPageNavigationService, INavigationService ServerPageNavigationService)
        {
            this.AccountStore = AccountStore;

            this.ServerAccountStore = ServerAccountStore;

            ServersAccountsStore = serversAccountsStore;
            
            this.CurrentServerStore = CurrentServerStore;

            this.MainPageServerNavigationStore = MainPageServerNavigationStore;

            this.ServerPageNavigationService = ServerPageNavigationService;

            VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);

            OpenModalCommaCommand=new LambdaCommand(OnOpenModalCommaExecuted);

            ConnectServerCommand=new AsyncLamdaCommand(OnConnectServerExecuted, (ex) => StatusMessage = ex.Message);

            DisconnectServerCommand = new LambdaCommand(OnDisconnectServerExecuted,CanDisconnectServerExecute);

            AccountLogoutCommand = new LambdaCommand(OnAccountLogoutExecuted);

            OpenSettingsCommand = new NavigationCommand(SettingsPageNavigationService);

            SaveServerCommand = new AsyncLamdaCommand(OnSaveServerCommandExecuted, (ex) => StatusMessage = ex.Message, CanSaveServerCommandExecute);

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

            if (!CheckServerStatus(SelectedServerAccount.SavedServer.ApiIp))
            {
                return;
            }

            if (selectedViewModel != default)
            {
                DisconectServer();
            }


            ILoginServices loginServices = new LoginSevices<ServerAccountStore>(ServerAccountStore);


            Account ServerAccount = new Account
            {
                Login = SelectedServerAccount.Account.Login
            };


            try
            {
                var AuthoriseStatus = await loginServices.Login(true, SelectedServerAccount.Account, SelectedServerAccount.SavedServer.ApiIp);

                if (!AuthoriseStatus)
                {
                    MessageBox.Show("Authorization error. You will be logged without account", "Сообщение");

                    await loginServices.Login(false, ServerAccount, ServernameOrIp);
                }
                else
                {
                    ServerAccount = SelectedServerAccount.Account;
                }
            }
            catch (Exception e)
            {

                await loginServices.Login(false, ServerAccount, ServernameOrIp);

            }


            bool ServerStatus = await Task.Run(() => CheckServerStatus(SelectedServerAccount.SavedServer.Ip));

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

            ILoginServices loginServices = new LoginSevices<ServerAccountStore>(ServerAccountStore);

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

              var AuthoriseStatus =  await loginServices.Login(true, DictionaryServerAccount.Account, newServer.ApiIp);

              if (!AuthoriseStatus)
              {
                  MessageBox.Show("Authorization error. You will be logged without account", "Сообщение");

                  await loginServices.Login(false, ServerAccount, ServernameOrIp);
              }
              else
              {
                  ServerAccount = DictionaryServerAccount.Account;
              }
            }
            catch (Exception e)
            {

                await loginServices.Login(false, ServerAccount, ServernameOrIp);

            }

            StatusMessage = "Check sever status. . .";

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

        #region Данные с окна


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
