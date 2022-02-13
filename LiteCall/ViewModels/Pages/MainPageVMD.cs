using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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

namespace LiteCall.ViewModels.Pages
{
    internal class MainPageVMD:BaseVMD
    {
        public MainPageVMD(AccountStore AccountStore)
        {
            this.AccountStore = AccountStore;
            VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);
            OpenModalCommaCommand=new LambdaCommand(OnOpenModalCommaExecuted);
            ConnectServerCommand=new LambdaCommand(OnConnectServerExecuted,CanConnectServerExecute);
            _CurrentServer = new Server();

        }


        #region Команды

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
                ServerAdress = String.Empty;

            }

        }


        public ICommand ConnectServerCommand { get; }

        private bool CanConnectServerExecute(object p) => true;
       

        private void OnConnectServerExecuted(object p)
        {
           if (CheckServerStatus(ServerAdress))
           {
               ModalStatus = false;
               selectedViewModel = new ServerVMD(AccountStore, CurrentServer);
               ServerAdress = String.Empty;
               VisibilitiStatus=Visibility.Visible;
           }
           else
           {
               ErrorHeight = 40;
           }

            
        }



        #endregion

        #region Данные с окна

        private double _ErrorHeight = 0;
        public double ErrorHeight
        {
            get => _ErrorHeight;
            set => Set(ref _ErrorHeight, value);
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


        private string _ServerAdress;

        public string ServerAdress
        {
            get => _ServerAdress;
            set => Set(ref _ServerAdress, value);
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

            string path = "http://"+ serverAdress + ":7999/ServerHost/";

            HttpWebRequest request;

            try
            {
                request = (HttpWebRequest) WebRequest.Create(path);
            }
            catch (UriFormatException e)
            {
                return false;
            }
            
            
            request.Timeout = 5000;

            try
            {
                request.GetResponse();
                _CurrentServer.IP = ServerAdress;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

    }
}
