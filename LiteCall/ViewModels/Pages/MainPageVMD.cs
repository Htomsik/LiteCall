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
using LiteCall.Views.Pages;

namespace LiteCall.ViewModels.Pages
{
    internal class MainPageVMD:BaseVMD
    {
        public MainPageVMD(AccountStore AccountStore)
        {
            _Account = AccountStore.CurrentAccount;
            VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);
            OpenModalCommaCommand=new LambdaCommand(OnOpenModalCommaExecuted);
            ConnectServerCommand=new LambdaCommand(OnConnectServerExecuted,CanConnectServerExecute);
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
            ModalStatus = true;
            
        }


        public ICommand ConnectServerCommand { get; }

        private bool CanConnectServerExecute(object p) => true;

        private void OnConnectServerExecuted(object p)
        {
            ModalStatus = CheckServerStatus(ServerAdress);
        }



        #endregion



        #region Данные с окна


        private bool _ModalStatus;

        public bool ModalStatus
        {
            get => _ModalStatus;
            set => Set(ref _ModalStatus, value);
        }


        private Account _Account;

        public Account Account
        {
            get => _Account;
            set => Set(ref _Account, value);
        }


        private string _ServerAdress;

        public string ServerAdress
        {
            get => _ServerAdress;
            set => Set(ref _ServerAdress, value);
        }


        public ObservableCollection<Server> ServersMark { get; }
        private Server _CurrentServer;
        public Server CurrentServer
        {
            get => _CurrentServer;
            set => Set(ref _CurrentServer, value);
        }


        private Visibility _VisibilitiStatus = Visibility.Collapsed;
        public Visibility VisibilitiStatus
        {
            get => _VisibilitiStatus;
            set => Set(ref _VisibilitiStatus, value);
        }


        private UserControl _selectedViewModel;
        public UserControl selectedViewModel
        {
            get => _selectedViewModel;
            set => Set(ref _selectedViewModel, value);
        }

        #endregion


        bool CheckServerStatus(string serverAdress)
        {
            string path = "http://"+ serverAdress + "/WPFHost/";
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(path);
            request.Timeout = 1000;

            try
            {
                request.GetResponse();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

    }
}
