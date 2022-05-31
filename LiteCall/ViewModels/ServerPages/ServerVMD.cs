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
using System.Windows;
using System.Windows.Documents;
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

    class DummyWaveProvider : IWaveProvider
    {
        public DummyWaveProvider(WaveFormat waveFormat)
        {
            WaveFormat = waveFormat;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public WaveFormat WaveFormat { get; }
    }
    internal class ServerVMD : BaseVMD
    {
        public ServerVMD(AccountStore _AccountStore, Server _CurrentServer)
        {

            #region Создание данных

            Account = _AccountStore.CurrentAccount;

            ServerService._AccountStore = _AccountStore;

            MessagesColCollection = new ObservableCollection<Message>();

            CurrentServer = _CurrentServer;

            #endregion

            //Инициализация подлючения
            InitSignalRConnection(CurrentServer);

            //Проверка на имя
            AsyncGetUserServerName();

            #region Шины сообщений

            //Сообщения
            MessageBus.Bus += AsyncGetMessageBUS;

            //Обновение комнат
            ReloadServerRooms.Reloader += AsynGetServerRoomsBUS;

            VoiceMessageBus.Bus += AsyncGetAudioBus;

            #endregion

            #region команды

            SendMessageCommand = new LambdaCommand(OnSendMessageExecuted, CanSendMessageExecuted);

            OpenModalCommand = new LambdaCommand(OnOpenModalCommandExecuted);

            CreateNewRoomCommand = new LambdaCommand(OnCreateNewRoomExecuted, CanCreateNewRoomExecute);

            ConnectCommand = new LambdaCommand(OnConnectExecuted, CanConnectExecute);

            DisconectGroupCommand = new LambdaCommand(OnDisconectGroupExecuted);

            VoiceInputCommand = new LambdaCommand(OnVoiceInputExecuted);

            #endregion

            #region Настройка Naduio

            input.DeviceNumber = 0;

            input.WaveFormat = new WaveFormat(8000, 16, 1);

            input.DataAvailable += Voice_Input;





            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)) { ReadFully = false };

            wave16ToFloatProvider = new Wave16ToFloatProvider(bufferedWaveProvider);

            _mixingWaveProvider32 = new MixingWaveProvider32(new[] { new DummyWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(8000, 1)), });


             _mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);

            _output = new WaveFloatTo16Provider(_mixingWaveProvider32);
            
            _waveOutEvent = new WaveOut();
            
            

            _waveOutEvent.Init(_output);








            #endregion

        }


        private BufferedWaveProvider bufferedWaveProvider;

        private Wave16ToFloatProvider wave16ToFloatProvider;

        private WaveOut _waveOutEvent;

        private MixingWaveProvider32 _mixingWaveProvider32;

        private WaveFloatTo16Provider _output;

        private WaveIn input = new WaveIn();


        #region Подключение к серверу


        /// <summary>
        /// Инициализация соединения
        /// </summary>
        private static void InitSignalRConnection(Server CurrentServer)
        {
            ServerService.ConnectionHub($"http://{CurrentServer.Ip}/LiteCall");

        }


        #endregion



        /// <summary>
        /// Включение/Выключение звука
        /// </summary>
        public ICommand VoiceInputCommand { get; }

        private void OnVoiceInputExecuted(object p)
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



        /// <summary>
        /// Выход из группы
        /// </summary>
        public ICommand DisconectGroupCommand { get; }

        private void OnDisconectGroupExecuted(object p)
        {
            CurrentGroup = null;
            AsyncGroupDisconect();
        }




        public ICommand CreateNewRoomCommand { get; }

        private bool CanCreateNewRoomExecute(object p) => !Convert.ToBoolean(p) && !string.IsNullOrEmpty(NewRoomName);

        private void OnCreateNewRoomExecuted(object p)
        {
            try
            {
                AsyncCreateRoom(NewRoomName);
                NewRoomName = string.Empty;
                ModalStatus = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
                NewRoomName = string.Empty;
            }


        }


        public ICommand ConnectCommand { get; }

        private bool CanConnectExecute(object p)
        {

            if (CurrentGroup is null) return true;

            if (p is ServerRooms)
            {
                var ConnectGroup = (ServerRooms)p;

                if (CurrentGroup.RoomName != ConnectGroup.RoomName)
                {
                    return true;
                }
            }
            return false;


        }

        private void OnConnectExecuted(object p)
        {

            AsyncConnectGroup((ServerRooms)p);
        }

        #region Отправка сообщений

        public ICommand SendMessageCommand { get; }

        public bool CanSendMessageExecuted(object p) => CurrentGroup is not null && !string.IsNullOrEmpty(CurrentMessage);

        private void OnSendMessageExecuted(object p)
        {


            Message newMessage = new Message
            {
                DateSend = DateTime.Now,
                Text = CurrentMessage,
                Sender = Account.CurrentServerLogin
            };

            AsyncSendMessage(newMessage);

            MessagesColCollection.Add(newMessage);

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






        List<string> users = new List<string>();
        public async void AsyncGetAudioBus(VoiceMessage newVoiceMes)
        {

            /*
            if (!users.Contains(newVoiceMes.Name))
            {
                _mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);
                users.Add(newVoiceMes.Name);
            }
          */

            //_mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);

            if (bufferedWaveProvider.BufferedBytes < 3200)
            {
                await Task.Factory.StartNew(() =>
                {
                    AsyncGetAudio(newVoiceMes);
                });
            }

            MessagesColCollection.Add(new Message
            {
                Text = bufferedWaveProvider.BufferedBytes.ToString()
            });

           


        }


        public void AsyncGetAudio(VoiceMessage newVoiceMes)
        {
          //   _mixingWaveProvider32.AddInputStream(wave16ToFloatProvider);

              var memoryStreamReader = new MemoryStream(newVoiceMes.AudioByteArray);

                byte[] buffer = new byte[100];


                bool readCompleted = false;

                while (!readCompleted)
                {

                    if (bufferedWaveProvider.BufferedBytes <= bufferedWaveProvider.BufferLength - buffer.Length*2)
                    {
                        int read = memoryStreamReader.Read(buffer, 0, buffer.Length);


                        if (read > 0)
                        {

                            bufferedWaveProvider.AddSamples(buffer, 0, read);
                            Thread.Sleep(0);
                            _waveOutEvent.Play();

                        }
                        else
                        {
                            readCompleted = true;
                        }
                    }

                    
            

          

                }
           

      //   _mixingWaveProvider32.RemoveInputStream(wave16ToFloatProvider);

       

        }




        /// <summary>
        /// Информация о комнатах и пользователях на сервере
        /// </summary>
        private async void AsynGetServerRoomsBUS()
        {
            var RoomListFromServer = await ServerService.hubConnection.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");
            ServerRooms = new ObservableCollection<ServerRooms>(RoomListFromServer);
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
                input.StopRecording();
                _waveOutEvent.Stop();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error:{e.Message}");
            }

        }

        /// <summary>
        /// Создание комнаты
        /// </summary>
        /// <param name="RoomName"></param>
        private async void AsyncCreateRoom(string _RoomName)
        {
            try
            {
                var GroupStatus = await ServerService.hubConnection.InvokeAsync<bool>("GroupCreate", _RoomName);


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
                MessageBox.Show($"Error:{e.Message}");

            }

        }

        /// <summary>
        /// Отправка сообщений
        /// </summary>
        /// <param name="RoomName"></param>
        private async void AsyncSendMessage(Message NewMessage)
        {
            try
            {
                await ServerService.hubConnection.InvokeAsync("SendMessage", NewMessage);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error:{e.Message}");
            }


        }

        /// <summary>
        /// Присоединение к комнате
        /// </summary>
        /// <param name="ConnectedGroup"></param>
        private async void AsyncConnectGroup(ServerRooms ConnectedGroup)
        {

            try
            {
                var ConnetGroupStatus = await ServerService.hubConnection.InvokeAsync<bool>("GroupConnect", $"{ConnectedGroup.RoomName}");

                if (ConnetGroupStatus)
                {
                    input.StartRecording();
                    CurrentGroup = ConnectedGroup;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error:{e.Message}");
            }

        }


        private async void AsyncGetUserServerName()
        {
            var NewName = string.Empty;
            try
            {
                NewName = await ServerService.hubConnection.InvokeAsync<string>("SetName", _Account.Login).ConfigureAwait(false);
            }
            catch (Exception e)
            {

            }

            //Если пришедшее имя содержит имя пользователя на клиенте то задаем его
            if (!string.IsNullOrEmpty(NewName) && NewName.Contains(Account.Login))
            {
                _Account.CurrentServerLogin = NewName;
            }



        }

        private async void Voice_Input(object sender, WaveInEventArgs e)
        {

           
            try
            {

                 if (ProcessData(e))
                 {
                    await ServerService.hubConnection.SendAsync("SendAudio", e.Buffer);
                 }
               
            }
            catch (Exception ex)
            {

            }

            
        }

        private bool ProcessData(WaveInEventArgs e)
        {

            double porog = 0.01;
            bool result = false;
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
            if (Tr || Sum2 > porog)
            { result = true; }
            else
            { result = false; }
            return result;
        }



        public override void Dispose()
        {
            MessageBus.Bus -= AsyncGetMessageBUS;

            ReloadServerRooms.Reloader -= AsynGetServerRoomsBUS;

            VoiceMessageBus.Bus -= AsyncGetAudioBus;

            input.StopRecording();

            _waveOutEvent.Stop();

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
