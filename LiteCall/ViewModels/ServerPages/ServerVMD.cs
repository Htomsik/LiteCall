using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.ApplicationServices;
using NAudio.Wave;
using SignalRServ;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerVMD:BaseVMD
    {
        public ServerVMD(AccountStore _AccountStore, Server _CurrentServer)
        {

            Account = _AccountStore.CurrentAccount;

            CurrentServer = _CurrentServer;


            #region Шины сообщений

            //Сообщения
            MessageBus.Bus += Receive;

            //Обновение комнат
            ReloadServerRooms.Reloader += GetServerRoomsAsync;

            #endregion


            MessagesColCollection = new ObservableCollection<Message>();


            //Инициализация подлючения
            InitSignalRConnection(CurrentServer);


           var NewName =  ServerService.hubConnection.InvokeAsync<string>("SetName", _Account.Login);

           
            #region команды

            SendMessageCommand = new LambdaCommand(OnSendMessageExecuted,CanSendMessageExecuted);

            OpenModalCommand = new LambdaCommand(OnOpenModalCommandExecuted);

            CreateNewRoomCommand = new LambdaCommand(OnCreateNewRoomExecuted, CanCreateNewRoomExecute);

            ConnectCommand = new LambdaCommand(OnConnectExecuted,CanConnectExecute);

            #endregion

        }

        #region То что относится к серверу



        /// <summary>
        /// Инициализация соединения
        /// </summary>
        private static void InitSignalRConnection(Server CurrentServer)
        {
            
            ServerService.ConnectionHub($"http://{CurrentServer.IP}:5000/LiteCall");
        }




        /// <summary>
        /// Информация о комнатах и пользователях на сервере
        /// </summary>
        private async void GetServerRoomsAsync()
        {

            var RoomListFromServer = await ServerService.hubConnection.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");


          ServerRooms = new ObservableCollection<ServerRooms>(RoomListFromServer);


        }





        #endregion

        





        public ICommand CreateNewRoomCommand { get; }

        private bool CanCreateNewRoomExecute(object p) => Convert.ToString(p)?.Length >=3;

        private void OnCreateNewRoomExecuted(object p)
        {

            AsyncCreateRoom((string)p);
            NewRoomName = string.Empty;
            ModalStatus = false;
           

        }

        private async void AsyncCreateRoom(string RoomName)
        {
            await ServerService.hubConnection.InvokeAsync("GroupCreate", RoomName);
        }




        public ICommand OpenModalCommand { get; }

       
        private void OnOpenModalCommandExecuted(object p)
        {

            if ((string)p == "1")
            {
                ModalStatus = true;
            }
            else
            {
                ModalStatus = false;
                NewRoomName=string.Empty;
            }
            

        }




        public ICommand ConnectCommand { get; }

        private bool CanConnectExecute(object p) => p is  ServerRooms;

        private void OnConnectExecuted(object p)
        {
            ServerRooms sr = (ServerRooms)p;

          //  ConnectRoom(sr.Name);
        }






        private ServerRooms _SelRooms;

        public ServerRooms SelRooms
        {
            get => _SelRooms;
            set => Set(ref _SelRooms, value);
        }






        public ICommand SendMessageCommand { get; }

        private void OnSendMessageExecuted(object p)
        {
            try
            {
             
                Message newMessage = new Message
                {
                    DateSend = DateTime.Now,
                    text = CurrentMessage,
                    User = Account.Login,
                    Type = false
                };

                MessagesColCollection.Add(newMessage);

                CurrentMessage = string.Empty;
                

            }
            catch (Exception e)
            {
               
            }
           
        }   

        private bool CanSendMessageExecuted(object p) => true;




        #region Методы

        #region Подписки

        private void Receive(Message lastmessage)
        {

          /*  App.Current.Dispatcher.Invoke((Action)delegate
            {
                MessagesColCollection.Add(lastmessage);
            });
          */
        }


        #endregion



        public static WaveOut output = new WaveOut();
        public static WaveIn input;
        public static BufferedWaveProvider bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)); 




        void ConnectRoom( string RoomName)
        {
            
            input = new WaveIn();
            input.DeviceNumber = 0;
            
            input.WaveFormat = new WaveFormat(8000, 16, 1);
          
            input.DataAvailable += Voice_Input;


            output.Init(bufferStream);


            input.StartRecording();
        }


        private  void Voice_Input(object sender, WaveInEventArgs e)
        {
            

          
        }




        public override void Dispose()
        {
            MessageBus.Bus -= Receive;

            ReloadServerRooms.Reloader -= GetServerRoomsAsync;

            base.Dispose();
        }

        #endregion


        #region Данные с формы
        public ObservableCollection<Message> MessagesColCollection { get; set; }

        private Server _CurrentServer;

        public Server CurrentServer
        {
            get => _CurrentServer;
            set => Set(ref _CurrentServer, value);
        }

        private string _CurrentMessage;

        public string CurrentMessage
        {
            get => _CurrentMessage;
            set => Set(ref _CurrentMessage, value);
        }


        private bool _ModalStatus;

        public bool ModalStatus
        {
            get => _ModalStatus;
            set => Set(ref _ModalStatus, value);
        }


        private string _NewRoomName;

        public string NewRoomName
        {
            get => _NewRoomName;
            set => Set(ref _NewRoomName, value);
        }



        private ObservableCollection<ServerRooms> _ServerRooms;

        public ObservableCollection<ServerRooms> ServerRooms
        {
            get => _ServerRooms;
            set => Set(ref _ServerRooms, value);
        }




        #endregion


        #region Хранилища

        private Account _Account;

        public Account Account
        {
            get => _Account;
            set => Set(ref _Account, value);
        }

        #endregion



       

    }

    
}
