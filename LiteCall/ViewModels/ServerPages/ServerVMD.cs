using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SignalRServ;


namespace LiteCall.ViewModels.ServerPages
{

  
    internal class ServerVMD : BaseVMD
    {
        public ServerVMD(ServerAccountStore _ServerAccountStore, CurrentServerStore _CurrentServerStore, IStatusServices statusServices)
        {

            #region Создание данных

            ServerAccountStore = _ServerAccountStore;

            MessagesColCollection = new ObservableCollection<Message>();

            CurrentServerStore = _CurrentServerStore;

            _statusServices = statusServices;

            #endregion

            InitSignalRConnection(CurrentServerStore.CurrentServer, ServerAccountStore.CurrentAccount);

            //Проверка на имя
            AsyncGetUserServerName();

            #region Шины сообщений

            //Сообщения
            MessageBus.Bus += AsyncGetMessageBUS;

            //Обновение комнат
            ReloadServerRooms.Reloader += AsynGetServerRoomsBUS;

            //Приход сообщений
            VoiceMessageBus.Bus += AsyncGetAudioBus;

            DisconnectNotification.Notificator += GroupDisconnected;


     


            #endregion

            #region команды

            SendMessageCommand = new AsyncLamdaCommand(OnSendMessageExecuted, ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanSendMessageExecuted);

            CreateNewRoomCommand = new AsyncLamdaCommand(OnCreateNewRoomExecuted, ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanCreateNewRoomExecute);

            ConnectCommand = new AsyncLamdaCommand(OnConnectExecuted, ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanConnectExecute);

            ConnectWithPasswordCommand = new AsyncLamdaCommand(OnConnectWithPasswordCommandExecuted,
                ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanConectWithPasswordExecute);

            OpenCreateRoomModalCommand = new LambdaCommand(OnOpenCreateRoomModalCommandExecuted);

            OpenPasswordModalCommand = new LambdaCommand(OnOpenPasswordModalCommandCommandExecuted);

            DisconectGroupCommand = new LambdaCommand(OnDisconectGroupExecuted);

      



            #region Админ команды

            AdminDeleteRoomCommand = new AsyncLamdaCommand(OnAdminDeleteRoomExecuted,
                ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanAdminDeleteRoomExecute);

            AdminDisconnectUserFromRoomCommand = new AsyncLamdaCommand(OnAdminDisconnectUserFromRoomExecuted,
                ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }), CanAdminDisconnectUserFromRoomExecute);

            #endregion


            #endregion

            #region Настройка Naduio

            input = new WaveIn();

            
            input.DataAvailable += InputDataAvailable;

            input.BufferMilliseconds = 10;

            input.WaveFormat = _waveFormat;


           // _playBuffer = new BufferedWaveProvider(_waveFormat);

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(8000, 1));

            mixer.ReadFully = true;

            _waveOut.DeviceNumber = 0;


            _waveOut.Init(mixer);

           

            

            /*

                        bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)) { ReadFully = false };

                        wave16ToFloatProvider = new Wave16ToFloatProvider(bufferedWaveProvider);

                        _mixingWaveProvider32 = new MixingWaveProvider32(new[] { new DummyWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(8000, 1)), });

                        _mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);

                        _output = new WaveFloatTo16Provider(_mixingWaveProvider32);

                        _waveOutEvent = new WaveOut(WaveCallbackInfo.FunctionCallback());

                        _waveOutEvent.Init(_output);

                        _waveOutEvent.Play();

            */
            #endregion



        }


        WaveIn input;

        private WaveOut _waveOut = new WaveOut();

        BufferedWaveProvider _playBuffer;

        MixingSampleProvider mixer;

        private WaveFormat _waveFormat = new WaveFormat(8000, 16, 1);


        #region Подключение к серверу


     
        public async void InitSignalRConnection(Server CurrentServer, Account CurrentAccount)
        {
            try
            {

                await ServerService.ConnectionHub($"https://{CurrentServer.Ip}/LiteCall", CurrentAccount, _statusServices);
                CanServerConnect = true;
            }
            catch (Exception e)
            {
                CanServerConnect = false;
            }
           

            _statusServices.DeleteStatus();
        }


        #endregion

      
      




        #region Админ команды


        public ICommand AdminDeleteRoomCommand { get; }

        private bool CanAdminDeleteRoomExecute(object p)
        {
            if (ServerAccountStore.CurrentAccount.Role != "Admin") return false;

            if (p is not Model.ServerRooms) return false;

            if (p == null) return false;

            return true;
        }

       private async Task OnAdminDeleteRoomExecuted(object p)
       {
             var deletedRoom = (ServerRooms)p;

            try
            {
                await ServerService.hubConnection.SendAsync("AdminDeleteRoom", deletedRoom.RoomName);
            }
            catch (Exception ex){}
            
       }


        public ICommand AdminDisconnectUserFromRoomCommand { get; }

        private bool CanAdminDisconnectUserFromRoomExecute(object p)
        {
            if (ServerAccountStore.CurrentAccount.Role != "Admin") return false;

            if (p is not ServerUser) return false;

            if (p == null) return false;

            return true;
        }

        private async Task OnAdminDisconnectUserFromRoomExecuted(object p)
        {
            var disconnectedUser = (ServerUser)p;

            try
            {
                await ServerService.hubConnection.SendAsync("AdminKickUser", disconnectedUser.Login);
            }
            catch (Exception ex) { }
        }

        #endregion





        /// <summary>
        /// Выход из группы
        /// </summary>
        public ICommand DisconectGroupCommand { get; }

        private void OnDisconectGroupExecuted(object p)
        {
            AsyncGroupDisconect();
        }



        public ICommand CreateNewRoomCommand { get; }

        private bool CanCreateNewRoomExecute(object p) => !Convert.ToBoolean(p) && !string.IsNullOrEmpty(NewRoomName);

        private async Task OnCreateNewRoomExecuted(object p)
        {
            
                try
                {
                    var GroupStatus = await ServerService.hubConnection.InvokeAsync<bool>("GroupCreate", NewRoomName, NewRoomPassword);

                    if (GroupStatus)
                    {

                        CurrentGroup = new ServerRooms
                        {
                            RoomName = NewRoomName,
                            Users = await ServerService.hubConnection.InvokeAsync<List<ServerUser>>("GetUsersRoom", NewRoomName)
                        };

                    }

                }
                catch (Exception e)
                {
                    

                }

                NewRoomName = string.Empty;
                NewRoomPassword = string.Empty;
                CreateRoomModalStatus = false;
        }





        public ICommand OpenCreateRoomModalCommand { get; }

        private void OnOpenCreateRoomModalCommandExecuted(object p)
        {

            if ((string)p == "1")
            {
                CreateRoomModalStatus = true;
            }
            else
            {
                CreateRoomModalStatus = false;
                NewRoomName = string.Empty;
                NewRoomPassword=string.Empty;
            }


        }






        public ICommand OpenPasswordModalCommand { get; }

        private void OnOpenPasswordModalCommandCommandExecuted(object p)
        {

            if ((string)p == "1")
            {
                RoomPasswordModalStatus = true;
            }
            else
            {
                RoomPasswordModalStatus = false;
                RoomPassword = string.Empty;
            }


        }




        public ICommand ConnectCommand { get; }

        private bool CanConnectExecute(object p)
        {
            if (p is not Model.ServerRooms) return false;
                
            if (p == null) return false;

            var ConnectedGroup = (ServerRooms)p;

            if (CurrentGroup is not null)
            {
                return ConnectedGroup.RoomName.ToLower() != CurrentGroup.RoomName.ToLower();
            }
            else
            {
                return true;
            }
           
           
        }

        private async Task OnConnectExecuted(object p)
        {

            var ConnectedGroup = (ServerRooms)p;


            if (ConnectedGroup.Guard)
            {
                RoomPasswordModalStatus = true;
                return;
            }
            else
            {
                await AsyncConnectCommand(ConnectedGroup);
            }

          
        }

        public ICommand ConnectWithPasswordCommand { get; }

        private bool CanConectWithPasswordExecute(object p) => !Convert.ToBoolean(p) && !string.IsNullOrEmpty(RoomPassword);

        private async Task OnConnectWithPasswordCommandExecuted(object p)
        {
            await AsyncConnectCommand(SelRooms, RoomPassword);
            RoomPassword = string.Empty;
            RoomPasswordModalStatus = false;

        }



        async Task AsyncConnectCommand(ServerRooms ConnectedGroup,string RoomPassword = null)
        {
            try
            {
                var ConnetGroupStatus = await ServerService.hubConnection.InvokeAsync<bool>("GroupConnect",
                    $"{ConnectedGroup.RoomName}", RoomPassword);

                if (ConnetGroupStatus)
                {
                    CurrentGroup = ConnectedGroup;
                    _waveOut.Play();
                    MicophoneMute = false;
                }
            }
            catch (Exception e)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed connect to the room", isError = true });
            }
        }



        #region Отправка сообщений

        public ICommand SendMessageCommand { get; }

        public bool CanSendMessageExecuted(object p) => CurrentGroup is not null && !string.IsNullOrEmpty(CurrentMessage);

        private async Task OnSendMessageExecuted(object p)
        {

            Message newMessage = new Message
            {
                DateSend = DateTime.Now,
                Text = CurrentMessage,
                Sender = ServerAccountStore.CurrentAccount.CurrentServerLogin
            };

            try
            {
                await ServerService.hubConnection.InvokeAsync("SendMessage", newMessage);
                MessagesColCollection.Add(newMessage);
            }
            catch (Exception e)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed send message", isError = true });
            }

            CurrentMessage = string.Empty;

        }



        #endregion

        #region Методы

        #region Подписки


        /// <summary>
        /// Приём сообщений
        /// </summary>
        /// <param name=""></param>
        private void AsyncGetMessageBUS(Message newMessage)
        {

            MessagesColCollection.Add(newMessage);
            
        }

        private Dictionary<string, BufferedWaveProvider> bufferUsers = new Dictionary<string, BufferedWaveProvider>();

        public async  void AsyncGetAudioBus(VoiceMessage newVoiceMes)
        {

            if (HeadphoneMute) return;

            try
            {
                var userbuffer = bufferUsers[newVoiceMes.Name];

                userbuffer.AddSamples(newVoiceMes.AudioByteArray, 0, newVoiceMes.AudioByteArray.Length);

                
            }
            catch (Exception e)
            {

                try
                {
                    bufferUsers.Add(newVoiceMes.Name, new BufferedWaveProvider(_waveFormat));

                    var userb1uffer = bufferUsers[newVoiceMes.Name];

                    mixer.AddMixerInput(userb1uffer);
                }
                catch (Exception exception){}
                
            }


        }


        /// <summary>
        /// Информация о комнатах и пользователях на сервере
        /// </summary>
        private async void AsynGetServerRoomsBUS()
        {

            try
            {
                var RoomListFromServer = await ServerService.hubConnection.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");


                ServerRooms = new ObservableCollection<ServerRooms>(RoomListFromServer);


            }
            catch (Exception e)
            {
                ServerRooms = new ObservableCollection<ServerRooms>();
            }
            
        }


        #endregion


        /// <summary>
        /// Выход из комнаты
        /// </summary>
        private async void AsyncGroupDisconect()
        {
            try
            {
                await ServerService.hubConnection.InvokeAsync("GroupDisconnect");
                GroupDisconnected();
            }
            catch (Exception e)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed disconnect from group", isError = true });

            }

        }

        private void GroupDisconnected()
        {

            mixer.RemoveAllMixerInputs();

            bufferUsers.Clear();

            CurrentGroup = null;

            MessagesColCollection = new ObservableCollection<Message>();

            _waveOut.Stop();

            MicophoneMute = false;
        }

        
        private async void AsyncGetUserServerName()
        {
            var NewName = string.Empty;
            try
            {
                NewName = await ServerService.hubConnection.InvokeAsync<string>("SetName", ServerAccountStore.CurrentAccount.Login).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Dispose();
            }

            if (NewName == "non")
            {
                Dispose();
            }
            
            ServerAccountStore.CurrentAccount.CurrentServerLogin = NewName;
            

        }

        private async  void InputDataAvailable(object sender, WaveInEventArgs e)
        {

            try
            {

                if (CurrentGroup != null)
                {
                        if (VAD(e))
                            await ServerService.hubConnection.SendAsync("SendAudio", e.Buffer);

                }

            }
            catch (Exception ex) { }

        }

        private bool VAD(WaveInEventArgs e)
        {
            double porog = 0.005;

            bool Tr = false;

            double Sum2 = 0;
            int Count = e.BytesRecorded / 2;


            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                double Tmp = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);

                Tmp /= 32768.0;

                Sum2 += Tmp * Tmp;

                if (Tmp > porog)

                    Tr = true;
            }

            Sum2 /= Count;

            return Tr || Sum2 > porog;
        }


        public override void Dispose()
        {
            ServerService.hubConnection.StopAsync();

            MessageBus.Bus -= AsyncGetMessageBUS;

            ReloadServerRooms.Reloader -= AsynGetServerRoomsBUS;

            VoiceMessageBus.Bus -= AsyncGetAudioBus;

            input.StopRecording();

            _waveOut.Stop();

            CurrentServerStore.Delete();

            ServerAccountStore.Logout();

            base.Dispose();
        }


        #endregion


        #region Данные с формы
        private ObservableCollection<Message> _messagesColCollection;

        public ObservableCollection<Message> MessagesColCollection
        {
            get => _messagesColCollection;
            set => Set(ref _messagesColCollection, value);
        }



        private CurrentServerStore _currentServerStore;

        public CurrentServerStore CurrentServerStore
        {
            get => _currentServerStore;
            set => Set(ref _currentServerStore, value);
        }


        private string _currentMessage;

        public string CurrentMessage
        {
            get => _currentMessage;
            set => Set(ref _currentMessage, value);
        }


        private bool _createRoomModalStatus;

        public bool CreateRoomModalStatus
        {
            get => _createRoomModalStatus;
            set => Set(ref _createRoomModalStatus, value);
        }


        private string _newRoomName;

        public string NewRoomName
        {
            get => _newRoomName;
            set => Set(ref _newRoomName, value);
        }



        private string _newRoomPassword;

        public string NewRoomPassword
        {
            get => _newRoomPassword;
            set => Set(ref _newRoomPassword, value);
        }




        private bool _roomPasswordModalStatus;

        public bool RoomPasswordModalStatus
        {
            get => _roomPasswordModalStatus;
            set => Set(ref _roomPasswordModalStatus, value);
        }


        private string _roomPassword;

        public string RoomPassword
        {
            get => _roomPassword;
            set => Set(ref _roomPassword, value);
        }

        private ServerRooms _selRooms;

        public ServerRooms SelRooms
        {
            get => _selRooms;
            set => Set(ref _selRooms, value);
        }


        private ServerUser _selServerUser;

        public ServerUser SelServerUser
        {
            get => _selServerUser;
            set => Set(ref _selServerUser, value);
        }



        private ObservableCollection<ServerRooms> _serverRooms;

        public ObservableCollection<ServerRooms> ServerRooms
        {
            get => _serverRooms;
            set
            {
                Set(ref _serverRooms, OnCurrentGoupChanged((ObservableCollection<ServerRooms>)value));
            }
        }



        private ServerRooms _currentGroup;

        public ServerRooms CurrentGroup
        {
            get => _currentGroup;
            set
            {
                Set(ref _currentGroup,value);
                
            }
        }

        private ObservableCollection<ServerRooms> OnCurrentGoupChanged(ObservableCollection<ServerRooms> CurrentRoomUsers)
        {
            foreach (var rooms in CurrentRoomUsers)
            {
                foreach (var users in rooms.Users)
                {
                    if (users.Login == ServerAccountStore.CurrentAccount.CurrentServerLogin)
                    {
                        users.Role = "You";
                    }
                }
            }


            return CurrentRoomUsers;
        }

        private bool _canServerConnect;
        public bool CanServerConnect
        {
            get => _canServerConnect;
            set => Set(ref _canServerConnect, value);
        }


        private bool _headphoneMute;

        public bool HeadphoneMute
        {
            get => _headphoneMute;
            set => Set(ref _headphoneMute, value);
        }


        private bool _micophoneMute;

        public bool MicophoneMute
        {
            get => _micophoneMute;
            set
            {
                Set(ref _micophoneMute, value);
                OmMicrophoneMuteChanged();
            }
        }


        void OmMicrophoneMuteChanged()
        {
            if (MicophoneMute)
            {
                try
                {
                    input.StopRecording();
                }
                catch (Exception e)
                {
                  
                }
               
            }
            else
            {
                try
                {
                    input.StartRecording();
                   
                }
                catch (Exception e)
                {

                }

            }
        }

        #endregion


        #region Хранилища

        public ServerAccountStore ServerAccountStore { get; set; }

        #endregion

        #region Сервисы

        private readonly IStatusServices _statusServices;

        #endregion





    }


}
