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

            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + this.CurrentServer.IP + ":7998/ServerHost/Chat");

            factory.Open();

            server = factory.CreateChannel();

            Reload();

        //    ServerRooms = GetServerRooms();

            GetServerInfo();

            //server.Join(_Account.Login, _Account.Password, false);

           


            #region команды

            SendMessageCommand = new LambdaCommand(OnSendMessageExecuted,CanSendMessageExecuted);

            #endregion

        }

        #region То что относится к серверу

        static InstanceContext context;
        static DuplexChannelFactory<IChatService> factory;
        static IChatService server;
        NetTcpBinding netTcpBinding;

        #endregion




        public ICommand OpenModalCommand { get; }

        private bool CanOpenModalExecute(object p) => true;

        private void OnOpenModalExecuted(object p)
        {

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


      

        private async void Reload()
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
            server.Join(_Account.Login, _Account.Password, false);

        }



        ObservableCollection<ServerRooms>? GetServerRooms()
        {
            var ServerRooms = new ObservableCollection<ServerRooms>();
            var ListRoom = server.GetRoomList();

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


            return Users;
        }


        void GetServerInfo()
        {
            var ServerInfo = server.ServerInfo();

            CurrentServer.Name = ServerInfo[0];

            CurrentServer.MaxUsers = Convert.ToInt16(ServerInfo[1]);

        }


        public override void Dispose()
        {
            MessageBus.Bus -= Receive;
            ReloadServer.Reloader -= Reload;
            server.SendMessage("Я", "Отключился");
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



        private TYPE _Name;

        public TYPE Name
        {
            get => _Name;
            set => Set(ref _Name, value);
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

        

        public static WaveIn input;
        //поток для речи собеседника
        public static WaveOut output;
        //буфферный поток для передачи через сеть
        public static BufferedWaveProvider bufferStream;

        
        public void RecievAudio(string user, byte[] message, bool type)
        {
            output.Play();
            //  Console.WriteLine(user);
            int received = message.Length;
            //   Console.WriteLine("Говорит:" + user);
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
