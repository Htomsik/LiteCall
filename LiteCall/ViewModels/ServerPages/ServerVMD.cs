using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioLib;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.VisualBasic.ApplicationServices;
using NAudio.Wave;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerVMD:BaseVMD
    {
        public ServerVMD(AccountStore AccountStore,Server CurrentServer)
        {
            
            _Account = AccountStore.CurrentAccount;

          this.CurrentServer= CurrentServer;

          MessagesColCollection = new ObservableCollection<Message>();

          MessageBus.Bus += Receive;

          ReloadServer.Reloader += Reload;
           

            context = new InstanceContext(new MyCallback());

            netTcpBinding = new NetTcpBinding(SecurityMode.None, true);

            
            Reload();

            GetServerInfo();

            
            #region команды

            SendMessageCommand = new LambdaCommand(OnSendMessageExecuted,CanSendMessageExecuted);

            OpenModalCommand = new LambdaCommand(OnOpenModalCommandExecuted);

            CreateNewRoomCommand = new LambdaCommand(OnCreateNewRoomExecuted, CanCreateNewRoomExecute);

            ReloadServerCommand =new LambdaCommand(OnReloadServerExecuted);

            #endregion

        }

        #region То что относится к серверу

        static InstanceContext context;
        static DuplexChannelFactory<IChatService> factory;
        static IChatService server;
        NetTcpBinding netTcpBinding;

        #endregion




        public ICommand ReloadServerCommand { get; }

        private void OnReloadServerExecuted(object p)
        {
            Reload();
        }


        public ICommand CreateNewRoomCommand { get; }

        private bool CanCreateNewRoomExecute(object p) => NewRoomName?.Length>3 ;

        private void OnCreateNewRoomExecuted(object p)
        {
            server.CreateRoom(NewRoomName);
            NewRoomName = string.Empty;
            ModalStatus = false;
           

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



        public ICommand SendMessageCommand { get; }

        private void OnSendMessageExecuted(object p)
        {
            try
            {
                server.SendMessage(CurrentMessage, Account.Login);
                Message newMessage = new Message
                {
                    Date = DateTime.Now,
                    Info = CurrentMessage,
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

        private bool CanSendMessageExecuted(object p) => CurrentMessage?.Length <= 2;




        #region Методы

        #region Подписки

        private void Receive(Message lastmessage)
        {

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                MessagesColCollection.Add(lastmessage);
            });

        }


      

        private void Reload()
        {

            ServerRooms = GetServerRooms();

        }

        #endregion


        


        void ConnectRoom( string RoomName)
        {
            factory.Close();
            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + CurrentServer.IP + ":7998/ServerHost/Chat/"+RoomName);
            factory.Open();
            server = factory.CreateChannel();




            //server.Join(_Account.Login, _Account.Password, false);

            factory.Close();
            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + addres + ":7998/ServerHost/Chat/" + RoomName);
            factory.Open();
            server = factory.CreateChannel();
           


         

            input = new WaveInEvent();
            input.DeviceNumber = 0;
            //определяем его формат - частота дискретизации 8000 Гц, ширина сэмпла - 16 бит, 1 канал - моно
            input.WaveFormat = new WaveFormat(8000, 16, 1);
            //добавляем код обработки нашего голоса, поступающего на микрофон
            input.DataAvailable += Voice_Input;
            //создаем поток для прослушивания входящего звука
            output = new WaveOutEvent();
            //создаем поток для буферного потока и определяем у него такой же формат как и потока с микрофона
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            //привязываем поток входящего звука к буферному потоку


        }



        output = new WaveOutEvent();
        //создаем поток для буферного потока и определяем у него такой же формат как и потока с микрофона
        bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));

        public static WaveInEvent input;

        private static void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
                server.SendAudo(e.Buffer, _Account.Login);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ObservableCollection<ServerRooms>? GetServerRooms()
        {

            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + this.CurrentServer.IP + ":7998/ServerHost/Chat");

            factory.Open();

            server = factory.CreateChannel();

            var ServerRooms = new ObservableCollection<ServerRooms>();

            var ListRoom = server.GetRoomList();

            factory.Close();

            foreach (var _RoomName in ListRoom)
            {
                ServerRooms NewServerRooms = new ServerRooms
                {
                    Name = _RoomName,
                    Users = GetUserInRooms(_RoomName)
                };
                ServerRooms.Add(NewServerRooms);
            }


            return ServerRooms;
        }


        ObservableCollection<ServerUser> GetUserInRooms(string RoomName)
        {

            DuplexChannelFactory<IChatService> factory;

            var Users = new ObservableCollection<ServerUser>();
            List<string> ListUser;


            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + CurrentServer.IP + ":7998/ServerHost/Chat/" + RoomName);

            factory.Open();

            server = factory.CreateChannel();

            ListUser = server.GetUserList();

            foreach (var _UserName in ListUser)
            {
                ServerUser newUser = new ServerUser
                {
                    Login = _UserName
                };
                Users.Add(newUser);
            }

            factory.Close();

            return Users;
        }


        void GetServerInfo()
        {


            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + this.CurrentServer.IP + ":7998/ServerHost/Chat");

            factory.Open();

            server = factory.CreateChannel();

            var ServerInfo = server.ServerInfo();

            factory.Close();

            CurrentServer.Name = ServerInfo[0];

            CurrentServer.MaxUsers = Convert.ToInt16(ServerInfo[1]);

        }


        public override void Dispose()
        {
            MessageBus.Bus -= Receive;
            ReloadServer.Reloader -= Reload;
            server.Disconect();
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


        public ObservableCollection<ServerRooms> ServerRooms { get; set; }

      

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

    class MyCallback : IChatClient
    {

        

        public static WaveOut output;
        
        public static BufferedWaveProvider bufferStream;

        
        public void RecievAudio(string user, byte[] message, bool type)
        {
            output.Play();
            Console.WriteLine(user);
            int received = message.Length;
            bufferStream.AddSamples(message, 0, received);
        }

        public void RecievMessage(string user, string message, bool type)
        {
            Message newMessage = new Message
            {
                Date = DateTime.Now,
                Info = message,
                User = user,
                Type = type
            };

            MessageBus.Send(newMessage);

        }

        public void Refresh()
        {
        // ReloadServer.Reload();

        }
    }
}
