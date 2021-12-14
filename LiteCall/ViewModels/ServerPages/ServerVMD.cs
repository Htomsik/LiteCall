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
        public ServerVMD(AccountStore AccountStore, Server CurrentServer)
        {
            
            _Account = AccountStore.CurrentAccount;

          this.CurrentServer= CurrentServer;

          MessagesColCollection = new ObservableCollection<Message>();

          MessageBus.Bus += Receive;


          

                         context = new InstanceContext(new MyCallback(output,bufferStream));
                     netTcpBinding = new NetTcpBinding(SecurityMode.None, true);

                      factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + this.CurrentServer.IP + ":7998/ServerHost/Chat");

                     factory.Open();

                   server = factory.CreateChannel();



                     if (server.GetRoomList().Count == 0)
                     {
                         server.CreateRoom("Chat1");
                         server.CreateRoom("Chat2");
                       }


                       Reload();
                       GetServerInfo();


            




            /*    var Serv = new ObservableCollection<ServerRooms>();
            Random rnd = new Random();

            for (int i = 0; i < rnd.Next(2,5); i++)
            {
                ServerRooms NewServerRooms = new ServerRooms
                {
                    Name = "Room"+i,
                    Users = new ObservableCollection<ServerUser>()
                };
                for (int j = 0; j < rnd.Next(0,7); j++)
                {
                    ServerUser newUser = new ServerUser
                    {
                        Login = "Test"
                    };
                    NewServerRooms.Users.Add(newUser);
                }

                Serv.Add(NewServerRooms);
                    
            }

            this.ServerRooms = Serv;

            CurrentServer.Name = "TestServer"; */






            #region команды

            SendMessageCommand = new LambdaCommand(OnSendMessageExecuted,CanSendMessageExecuted);

            OpenModalCommand = new LambdaCommand(OnOpenModalCommandExecuted);

            CreateNewRoomCommand = new LambdaCommand(OnCreateNewRoomExecuted, CanCreateNewRoomExecute);

            ReloadServerCommand =new LambdaCommand(OnReloadServerExecuted);

            ConnectCommand = new LambdaCommand(OnConnectExecuted,CanConnectExecute);

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




        public ICommand ConnectCommand { get; }

        private bool CanConnectExecute(object p) => p is  ServerRooms;

        private void OnConnectExecuted(object p)
        {
            ServerRooms sr = (ServerRooms)p;

            ConnectRoom(sr.Name);
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

        private bool CanSendMessageExecuted(object p) => true;




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


    



            ServerRooms = GetServerRooms(server.GetRoomList());
          
           

        }

        #endregion



        public static WaveOut output = new WaveOut();
        public static WaveIn input;
        public static BufferedWaveProvider bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)); 




        void ConnectRoom( string RoomName)
        {
            factory.Close();
            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://" + CurrentServer.IP + ":7998/ServerHost/Chat/"+RoomName);
            factory.Open();
            server = factory.CreateChannel();


            input = new WaveIn();
            input.DeviceNumber = 0;
            
            input.WaveFormat = new WaveFormat(8000, 16, 1);
          
            input.DataAvailable += Voice_Input;


            output.Init(bufferStream);

            server.Join(Account.Login, Account.Password, false);



            input.StartRecording();
        }


        private  void Voice_Input(object sender, WaveInEventArgs e)
        {
            

                server.SendAudo(e.Buffer, _Account.Login);
          
        }


        ObservableCollection<ServerRooms> GetServerRooms(List<string> serverRooms)
        {

           
            var ServerRooms = new ObservableCollection<ServerRooms>();

            foreach (var _RoomName in serverRooms)
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

            

            CurrentServer.Name = ServerInfo[0];

            CurrentServer.MaxUsers = Convert.ToInt16(ServerInfo[1]);

        }


        public override void Dispose()
        {
            MessageBus.Bus -= Receive;
          
            
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


        public  ObservableCollection<ServerRooms> ServerRooms { get; set; }

      

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

         public MyCallback(WaveOut output1, BufferedWaveProvider bufferStream1)
         {
            output=output1;
            bufferStream=bufferStream1;
         }


         public static WaveOut output = new WaveOut();
        
        public static BufferedWaveProvider bufferStream;

        
        public void RecievAudio(string user, byte[] message, bool type)
        {
            output.Play();
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
