using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
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
        public ServerVMD(ServerAccountStore _ServerAccountStore, CurrentServerStore _CurrentServerStore)
        {

            #region Создание данных

            ServerAccountStore = _ServerAccountStore;

            MessagesColCollection = new ObservableCollection<Message>();

            CurrentServerStore = _CurrentServerStore;

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

            SendMessageCommand = new AsyncLamdaCommand(OnSendMessageExecuted, (ex) => StatusMessage = ex.Message, CanSendMessageExecuted);

            CreateNewRoomCommand = new AsyncLamdaCommand(OnCreateNewRoomExecuted,(ex) => StatusMessage = ex.Message,CanCreateNewRoomExecute);

            ConnectCommand = new AsyncLamdaCommand(OnConnectExecuted, (ex) => StatusMessage = ex.Message, CanConnectExecute);

            ConnectWithPasswordCommand = new AsyncLamdaCommand(OnConnectWithPasswordCommandExecuted,
                (ex) => StatusMessage = ex.Message, CanConectWithPasswordExecute);

            OpenCreateRoomModalCommand = new LambdaCommand(OnOpenCreateRoomModalCommandExecuted);

            OpenPasswordModalCommand = new LambdaCommand(OnOpenPasswordModalCommandCommandExecuted);

            DisconectGroupCommand = new LambdaCommand(OnDisconectGroupExecuted);

            VoiceInputCommand = new LambdaCommand(OnVoiceInputExecuted);



            #region Админ команды

            AdminDeleteRoomCommand = new AsyncLamdaCommand(OnAdminDeleteRoomExecuted,
                (ex) => StatusMessage = ex.Message, CanAdminDeleteRoomExecute);

            AdminDisconnectUserFromRoomCommand = new AsyncLamdaCommand(OnAdminDisconnectUserFromRoomExecuted,
                (ex) => StatusMessage = ex.Message, CanAdminDisconnectUserFromRoomExecute);

            #endregion


            #endregion

            #region Настройка Naduio




            input = new WaveIn();

            
            input.DataAvailable += InputDataAvailable;

            input.BufferMilliseconds = 20;

            input.WaveFormat = _waveFormat;


            _playBuffer = new BufferedWaveProvider(_waveFormat);

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(8000, 1));

            mixer.ReadFully = true;

            _waveOut.DeviceNumber = 0;

            mixer.AddMixerInput(_playBuffer);

            _waveOut.Init(mixer);

            _waveOut.Play();

            

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


        /// <summary>
        /// Инициализация соединения
        /// </summary>
        public async void InitSignalRConnection(Server CurrentServer, Account CurrentAccount)
        {
            StatusMessage = "Connecting to server. . .";

           await ServerService.ConnectionHub($"https://{CurrentServer.Ip}/LiteCall", CurrentAccount);

            StatusMessage = string.Empty;
        }


        #endregion

        /// <summary>
        /// Включение/Выключение звука
        /// </summary>
        public ICommand VoiceInputCommand { get; }

        private  void OnVoiceInputExecuted(object p)
        {
            if (!(bool)p)
            {
                try
                {
                    input.StartRecording();
                }
                catch (Exception e)
                {

                }

            }
            else
            {
                try
                {
                    input.StopRecording();
                }
                catch (Exception e)
                {

                }

            }
        }




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
            var deletedRoom = (ServerRooms)p;

            try
            {
                await ServerService.hubConnection.SendAsync("AdminKickFromRoomUser", deletedRoom.RoomName);
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
                    MessageBox.Show($"Error:{e.Message}");

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
                   
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error:{e.Message}");
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
                MessageBox.Show($"Error:{e.Message}");
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



        public async void AsyncGetAudioBus(VoiceMessage newVoiceMes)
        {

            
            // _playBuffer.AddSamples(newVoiceMes.AudioByteArray, 0, newVoiceMes.AudioByteArray.Length);


            StatusMessage = _playBuffer.BufferedBytes.ToString();

        
            _playBuffer.AddSamples(newVoiceMes.AudioByteArray, 0, newVoiceMes.AudioByteArray.Length);
            
          


        }

        /*
        public async Task AsyncGetAudio(VoiceMessage newVoiceMes)
        {
        // _mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);

              var memoryStreamReader = new MemoryStream(newVoiceMes.AudioByteArray);

                byte[] buffer = new byte[180];

                bool readCompleted = false;

                while (!readCompleted)
                {

                    if (bufferedWaveProvider.BufferedBytes <= bufferedWaveProvider.BufferLength - buffer.Length*2)
                    {
                        int read =  memoryStreamReader.Read(buffer, 0, buffer.Length);


                        if (read > 0)
                        {

                            bufferedWaveProvider.AddSamples(buffer, 0, read);
                            Thread.Sleep(0);
                        
                        }
                        else
                        {
                            readCompleted = true;
                        }
                    }

                    
            

          

                }
           

       //_mixingWaveProvider32.RemoveInputStream(wave16ToFloatProvider);

       

        }
        */

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
                MessageBox.Show($"Error:{e.Message}");
            }

        }

        private void GroupDisconnected()
        {
            CurrentGroup = null;
            MessagesColCollection = null;
            _waveOut.Stop();
            input.StopRecording();
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

            }

            //Если пришедшее имя содержит имя пользователя на клиенте то задаем его
            if (!string.IsNullOrEmpty(NewName) && NewName.Contains(ServerAccountStore.CurrentAccount.Login))
            {
                ServerAccountStore.CurrentAccount.CurrentServerLogin = NewName;
            }

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
                catch (Exception ex)
                {

                }

        }

        private bool VAD(WaveInEventArgs e)
        {
            double porog = 0.03;

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

            CurrentServerStore.CurrentServer = null;

            ServerAccountStore.CurrentAccount= null;

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



        private CurrentServerStore _CurrentServerStore;

        public CurrentServerStore CurrentServerStore
        {
            get => _CurrentServerStore;
            set => Set(ref _CurrentServerStore, value);
        }


        private string _CurrentMessage;

        public string CurrentMessage
        {
            get => _CurrentMessage;
            set => Set(ref _CurrentMessage, value);
        }


        private bool _createRoomModalStatus;

        public bool CreateRoomModalStatus
        {
            get => _createRoomModalStatus;
            set => Set(ref _createRoomModalStatus, value);
        }


        private string _NewRoomName;

        public string NewRoomName
        {
            get => _NewRoomName;
            set => Set(ref _NewRoomName, value);
        }



        private string _NewRoomPassword;

        public string NewRoomPassword
        {
            get => _NewRoomPassword;
            set => Set(ref _NewRoomPassword, value);
        }




        private bool _RoomPasswordModalStatus;

        public bool RoomPasswordModalStatus
        {
            get => _RoomPasswordModalStatus;
            set => Set(ref _RoomPasswordModalStatus, value);
        }


        private string _RoomPassword;

        public string RoomPassword
        {
            get => _RoomPassword;
            set => Set(ref _RoomPassword, value);
        }

        private ServerRooms _SelRooms;

        public ServerRooms SelRooms
        {
            get => _SelRooms;
            set => Set(ref _SelRooms, value);
        }


        private ServerUser _SelServerUser;

        public ServerUser SelServerUser
        {
            get => _SelServerUser;
            set => Set(ref _SelServerUser, value);
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

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);






        #endregion


        #region Хранилища

        public ServerAccountStore ServerAccountStore;

        #endregion





    }


}
