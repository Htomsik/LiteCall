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
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.ServerPages;
using LiteCall.Views.Pages;
using SignalRServ;

namespace LiteCall.ViewModels.Pages
{
    internal class MainPageVMD:BaseVMD
    {
        public MainPageVMD(AccountStore AccountStore)
        {
            this.AccountStore = AccountStore;

            VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);

            OpenModalCommaCommand=new LambdaCommand(OnOpenModalCommaExecuted);

            ConnectServerCommand=new LambdaCommand(OnConnectServerExecuted);

            DisconnectServerCommand = new LambdaCommand(OnDisconnectServerExecuted,CanDisconnectServerExecute);

            AccountLogoutCommand = new LambdaCommand(OnAccountLogoutExecuted,CanAccountLogoutExecute);

            CurrentServer = new Server();


            _savedServerCollection = new ObservableCollection<Server>
            {
              
            };
        }


        #region Команды

        public ICommand DisconnectServerCommand { get; }

        private bool CanDisconnectServerExecute(object p) => true;

        private void OnDisconnectServerExecuted(object p)
        {
            selectedViewModel.Dispose();
            selectedViewModel = null;
            CurrentServer.Ip = string.Empty;
            VisibilitiStatus = Visibility.Collapsed;
            ServerService.hubConnection.StopAsync();

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
                ErrorHeight = 0;
                ModalStatus = true;
            }
            else
            {
                ModalStatus = false;
                ServernameOrIp = String.Empty;

            }

        }



        public ICommand AccountLogoutCommand { get; }

        private bool CanAccountLogoutExecute(object p) => true;

        private void OnAccountLogoutExecuted(object p)
        {
            if (selectedViewModel!=null)
            {
                selectedViewModel.Dispose();
                selectedViewModel = null;
            }
            
            CurrentServer.Ip = string.Empty;
            VisibilitiStatus = Visibility.Collapsed;
            this.AccountStore.Logout();
        }

      


        public ICommand ConnectServerCommand { get; }

        private void OnConnectServerExecuted(object p)
        {


            Server newServer = new Server{Ip = ServernameOrIp};

            
            //по имени на сервере
            if (!CheckStatus)
            {
                newServer = DataBaseService.ServerGetInfo(ServernameOrIp).Result;
            }
            
            if (newServer is not null && CheckServerStatus(newServer.Ip))
            {
                CurrentServer = newServer;
               ModalStatus = false;
               selectedViewModel = new ServerVMD(AccountStore, CurrentServer);
               ServernameOrIp = String.Empty;
               VisibilitiStatus=Visibility.Visible;
              
            }
           else
           {
               ErrorHeight = 50;
           }
          


        }



        #endregion

        #region Данные с окна




        private ObservableCollection<Server> _savedServerCollection;

        public ObservableCollection<Server> savedServerCollection
        {
            get => _savedServerCollection;
            set => Set(ref _savedServerCollection, value);
        }



        private double _ErrorHeight = 0;
        public double ErrorHeight
        {
            get => _ErrorHeight;
            set => Set(ref _ErrorHeight, value);
        }

        private bool _CheckStatus;
        public bool CheckStatus
        {
            get => _CheckStatus;
            set => Set(ref _CheckStatus, value);
        }

        private Server _CurrentServer;

        public Server CurrentServer
        {
            get => _CurrentServer;
            set => Set(ref _CurrentServer, value);
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


        private BaseVMD _selectedViewModel;
        public BaseVMD selectedViewModel
        {
            get => _selectedViewModel;
            set => Set(ref _selectedViewModel, value);
        }

    
        #endregion


        bool CheckServerStatus(string serverAdress)
        {

            string[] ServerAddresArray = serverAdress.Split(':');

            if (ServerAddresArray.Length ==2)
            {
                try
                {
                    using (var client = new TcpClient(ServerAddresArray[0], Convert.ToInt32(ServerAddresArray[1])))
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

                return false;

            }

           

        }



    }
}
