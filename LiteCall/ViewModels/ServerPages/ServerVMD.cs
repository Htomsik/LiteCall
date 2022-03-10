using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
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

            ServerService._AccountStore = _AccountStore;




            CurrentServer = _CurrentServer;


            #region Шины сообщений

            //Сообщения
            MessageBus.Bus += AsyncGetMessageBUS;

            //Обновение комнат
            ReloadServerRooms.Reloader += AsynGetServerRoomsBUS;

            VoiceMessageBus.Bus += AsyncGetAudioBUS;

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

            DisconectGroupCommand = new LambdaCommand(OnDisconectGroupExecuted, CanDisconectGroupExecute);



            #region Настройка Naduio

            input = new WaveIn();
            input.DeviceNumber = 0;

            input.WaveFormat = new WaveFormat(8000, 16, 1);

            input.DataAvailable += Voice_Input;


            output.Init(bufferStream);
            output.DesiredLatency = 100;








            var wave16ToFloatProvider = new Wave16ToFloatProvider(bufferStream);

            _mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);

        

            #endregion







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


        #endregion





        public ICommand DisconectGroupCommand { get; }

        private bool CanDisconectGroupExecute(object p) => true;

        private void OnDisconectGroupExecuted(object p)
        {
            AsyncGroupDisconect();
        }




        public ICommand CreateNewRoomCommand { get; }

        private bool CanCreateNewRoomExecute(object p) => Convert.ToString(p)?.Length >=3;

        private void OnCreateNewRoomExecuted(object p)
        {
            try
            {
                AsyncCreateRoom((string)p);
                NewRoomName = string.Empty;
                ModalStatus = false;
            }
            catch (Exception e)
            {
                
            }
          
           

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

        private bool CanConnectExecute(object p) => p is ServerRooms;

        private void OnConnectExecuted(object p)
        {

            AsyncConnectGroup((ServerRooms)p);
        }



        #region Отправка сообщений

        public ICommand SendMessageCommand { get; }

        private void OnSendMessageExecuted(object p)
        {
          

                Message newMessage = new Message
                {
                    DateSend = DateTime.Now,
                    Text = CurrentMessage,
                    Sender = Account.Login,
                };

                AsyncSendMessage(newMessage);

                MessagesColCollection.Add(newMessage);

                CurrentMessage = string.Empty;


           

        }

        private bool CanSendMessageExecuted(object p) => true;

        #endregion





        #region Методы

        #region Подписки


        /// <summary>
        /// Приём сообщений
        /// </summary>
        /// <param name=""></param>
        private void AsyncGetMessageBUS(Message newMessage )
        {

            MessagesColCollection.Add(newMessage);


        }

        /// <summary>
        /// Получение звука
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public  void AsyncGetAudioBUS(VoiceMessage newVoiceMes)
        {




            bufferStream.AddSamples(newVoiceMes.AudioByteArray, 0, newVoiceMes.AudioByteArray.Length);

            Thread.Sleep(0);
            output.Play();
            

        }


        private  MixingWaveProvider32 _mixingWaveProvider32 = new MixingWaveProvider32();
      


        public static WaveOut output = new WaveOut();
        public static WaveIn input;
        public static BufferedWaveProvider bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));


      



        /// <summary>
        /// Информация о комнатах и пользователях на сервере
        /// </summary>
        private async void AsynGetServerRoomsBUS()
        {
            var RoomListFromServer = await ServerService.hubConnection.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");
            ServerRooms = new ObservableCollection<ServerRooms>(RoomListFromServer);
        }


        #endregion






        private async void AsyncGroupDisconect()
        {
            await ServerService.hubConnection.InvokeAsync("GroupDisconnect");
            input.StopRecording();
        }

        /// <summary>
        /// Создание комнаты
        /// </summary>
        /// <param name="RoomName"></param>
        private async void AsyncCreateRoom(string _RoomName)
        {
            try
            {
              var GroupStatus =  await ServerService.hubConnection.InvokeAsync<bool>("GroupCreate", _RoomName);


              if (GroupStatus)
              {

                  CurrentGroup = new ServerRooms
                  {
                      RoomName = _RoomName,
                      Users = await ServerService.hubConnection.InvokeAsync<List<ServerUser>>("GetUsersRoom", _RoomName)
                  };

                  input.StartRecording();
                 
              }

              
            }
            catch (Exception e)
            {
              
              
            }
           
        }

        /// <summary>
        /// Отправка сообщений
        /// </summary>
        /// <param name="RoomName"></param>
        private async void AsyncSendMessage(Message NewMessage)
        {
            await ServerService.hubConnection.InvokeAsync("SendMessage", NewMessage);
        }

        /// <summary>
        /// Присоединение к комнате
        /// </summary>
        /// <param name="ConnectedGroup"></param>
        private async void AsyncConnectGroup(ServerRooms ConnectedGroup)
        {
          var ConnetGroupStatus =  await ServerService.hubConnection.InvokeAsync<bool>("GroupConnect",$"{ConnectedGroup.RoomName}");

          if (ConnetGroupStatus)
          {
              input.StartRecording();
              CurrentGroup = ConnectedGroup;
            }
             



        }



        
        private async void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
               await ServerService.hubConnection.SendAsync("SendAudio", e.Buffer);
            }
            catch (Exception ex)
            {
               
            }
        }

       



        protected override void Dispose()
        {
            MessageBus.Bus -= AsyncGetMessageBUS;

            ReloadServerRooms.Reloader -= AsynGetServerRoomsBUS;

            VoiceMessageBus.Bus -= AsyncGetAudioBUS;

            base.Dispose();
        }


       

        #endregion


        #region Данные с формы
        private ObservableCollection<Message> _MessagesColCollection;

        public ObservableCollection<Message> MessagesColCollection
        {
            get => _MessagesColCollection;
            set => Set(ref _MessagesColCollection, value);
        }


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

        private ServerRooms _SelRooms;

        public ServerRooms SelRooms
        {
            get => _SelRooms;
            set => Set(ref _SelRooms, value);
        }


        private ObservableCollection<ServerRooms> _ServerRooms;

        public ObservableCollection<ServerRooms> ServerRooms
        {
            get => _ServerRooms;
            set => Set(ref _ServerRooms, value);
        }



        private ServerRooms _CurrentGroup;

        public ServerRooms CurrentGroup
        {
            get => _CurrentGroup;
            set => Set(ref _CurrentGroup, value);
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
