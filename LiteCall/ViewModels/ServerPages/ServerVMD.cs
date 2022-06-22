using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Bus;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LiteCall.ViewModels.ServerPages;

internal class ServerVMD : BaseVmd
{
    #region Services

    private readonly IStatusServices _statusServices;

    #endregion

    public ServerVMD(ServerAccountStore serverAccountStore, CurrentServerStore currentServerStore,
        IStatusServices statusServices)
    {
        _serverAccountStore = serverAccountStore;

        MessagesColCollection = new ObservableCollection<Message>();

        CurrentServerStore = currentServerStore;

        _statusServices = statusServices;

        InitSignalRConnection(CurrentServerStore.CurrentServer, _serverAccountStore.CurrentAccount);

        AsyncGetUserServerName();

        MessageBus.Bus += AsyncGetMessageBus;


        ReloadServerRooms.Reloader += AsynGetServerRoomsBus;

        VoiceMessageBus.Bus += AsyncGetAudioBus;

        DisconnectNotification.Notificator += GroupDisconnected;

        #region команды

        SendMessageCommand = new AsyncLambdaCommand(OnSendMessageExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanSendMessageExecuted);

        CreateNewRoomCommand = new AsyncLambdaCommand(OnCreateNewRoomExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanCreateNewRoomExecute);

        ConnectCommand = new AsyncLambdaCommand(OnConnectExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanConnectExecute);

        ConnectWithPasswordCommand = new AsyncLambdaCommand(OnConnectWithPasswordCommandExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanConnectWithPasswordExecute);

        OpenCreateRoomModalCommand = new LambdaCommand(OnOpenCreateRoomModalCommandExecuted);

        OpenPasswordModalCommand = new LambdaCommand(OnOpenPasswordModalCommandCommandExecuted);

        DisconnectGroupCommand = new LambdaCommand(OnDisconnectGroupExecuted);


        #region Админ команды

        AdminDeleteRoomCommand = new AsyncLambdaCommand(OnAdminDeleteRoomExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanAdminDeleteRoomExecute);

        AdminDisconnectUserFromRoomCommand = new AsyncLambdaCommand(OnAdminDisconnectUserFromRoomExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanAdminDisconnectUserFromRoomExecute);

        #endregion

        #endregion

        #region Naudio Settings

        _input = new WaveIn();

        _input.DataAvailable += InputDataAvailable!;

        _input.BufferMilliseconds = 10;

        _input.WaveFormat = _waveFormat;

        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(8000, 1))
        {
            ReadFully = true
        };

        _waveOut = new WaveOut();

        _waveOut.DeviceNumber = 0;

        _waveOut.Init(_mixer);

        _waveOut.Play();

        #endregion
    }

    #region Server hub connecting

    public async void InitSignalRConnection(Server? currentServer, Account? currentAccount)
    {
        try
        {
            await ServerService.ConnectionHub($"https://{currentServer!.Ip}/LiteCall", currentAccount, _statusServices);
            CanServerConnect = true;
        }
        catch
        {
            CanServerConnect = false;
        }


        _statusServices.DeleteStatus();
    }

    #endregion

    #region Stores

    public  CurrentServerStore CurrentServerStore { get; set; }

    private readonly ServerAccountStore _serverAccountStore;

    #endregion

    #region NAudio

    private readonly WaveFormat _waveFormat = new(8000, 16, 1);

    private readonly WaveOut _waveOut;

    private readonly WaveIn _input;

    private readonly MixingSampleProvider _mixer;

    #endregion

    #region Commands

    #region DiconnecFromGroup

    public ICommand DisconnectGroupCommand { get; }

    private void OnDisconnectGroupExecuted(object p)
    {
        AsyncGroupDisconect();
    }

    #endregion

    #region CreateNewRoom

    public ICommand CreateNewRoomCommand { get; }

    private bool CanCreateNewRoomExecute(object p)
    {
        return !Convert.ToBoolean(p) && !string.IsNullOrEmpty(NewRoomName);
    }

    private async Task OnCreateNewRoomExecuted(object p)
    {
        try
        {
            var groupStatus =
                await ServerService.HubConnection!.InvokeAsync<bool>("GroupCreate", NewRoomName, NewRoomPassword);

            if (groupStatus)
                CurrentGroup = new ServerRooms
                {
                    RoomName = NewRoomName,
                    Users = await ServerService.HubConnection!.InvokeAsync<List<ServerUser>>("GetUsersRoom", NewRoomName)
                };
        }
        catch 
        {
            // ignored
        }

        NewRoomName = string.Empty;
        NewRoomPassword = string.Empty;
        CreateRoomModalStatus = false;
    }

    #endregion

    #region SendMessage

    public ICommand SendMessageCommand { get; }

    public bool CanSendMessageExecuted(object p)
    {
        return CurrentGroup is not null && !string.IsNullOrEmpty(CurrentMessage);
    }

    private async Task OnSendMessageExecuted(object p)
    {
        var newMessage = new Message
        {
            DateSend = DateTime.Now,
            Text = CurrentMessage,
            Sender = _serverAccountStore.CurrentAccount!.CurrentServerLogin
        };

        try
        {
            await ServerService.HubConnection!.InvokeAsync("SendMessage", newMessage);
            MessagesColCollection!.Add(newMessage);
        }
        catch 
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Failed send message", IsError = true });
        }

        CurrentMessage = string.Empty;
    }

    #endregion

    #region OpenCreateRoomModal

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
            NewRoomPassword = string.Empty;
        }
    }

    #endregion

    #region ConnectRoom

    public ICommand ConnectCommand { get; }

    private bool CanConnectExecute(object p)
    {
        if (p is not ServerRooms rooms) return false;

        var ConnectedGroup = rooms;

        if (CurrentGroup is not null)
            return !String.Equals(ConnectedGroup.RoomName!, CurrentGroup.RoomName!, StringComparison.CurrentCultureIgnoreCase);
        return true;
    }

    private async Task OnConnectExecuted(object p)
    {
        var connectedGroup = (ServerRooms)p;

        if (connectedGroup.Guard)
        {
            RoomPasswordModalStatus = true;
            return;
        }

        await AsyncRoomConnect(connectedGroup);
    }

    #endregion

    #region OpenPasswordModal

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

    #endregion

    #region ConnectWithPasswordRooms

    public ICommand ConnectWithPasswordCommand { get; }

    private bool CanConnectWithPasswordExecute(object p)
    {
        return !Convert.ToBoolean(p) && !string.IsNullOrEmpty(RoomPassword);
    }

    private async Task OnConnectWithPasswordCommandExecuted(object p)
    {
        await AsyncRoomConnect(SelRooms, RoomPassword);
        RoomPassword = string.Empty;
        RoomPasswordModalStatus = false;
    }

    #endregion

    #region AdminDeleteRoom

    public ICommand AdminDeleteRoomCommand { get; }

    private bool CanAdminDeleteRoomExecute(object p)
    {
        if (_serverAccountStore.CurrentAccount!.Role != "Admin") return false;

        return p is ServerRooms;
    }

    private async Task OnAdminDeleteRoomExecuted(object p)
    {
        var deletedRoom = (ServerRooms)p;

        try
        {
            await ServerService.HubConnection!.SendAsync("AdminDeleteRoom", deletedRoom.RoomName);
        }
        catch
        {
            // ignored
        }
    }

    #endregion

    #region AdminKickUserFromRoom

    public ICommand AdminDisconnectUserFromRoomCommand { get; }

    private bool CanAdminDisconnectUserFromRoomExecute(object p)
    {
        if (_serverAccountStore.CurrentAccount!.Role != "Admin") return false;

        return p is ServerUser;
    }

    private async Task OnAdminDisconnectUserFromRoomExecuted(object p)
    {
        var disconnectedUser = (ServerUser)p;

        try
        {
            await ServerService.HubConnection!.SendAsync("AdminKickUser", disconnectedUser.Login);
        }
        catch
        {
            // ignored
        }
    }

    #endregion

    #endregion

    #region Methods

    #region ControlMethods

    private async Task AsyncRoomConnect(ServerRooms? ConnectedGroup, string? RoomPassword = "")
    {
        try
        {
            var connectRoomStatus = await ServerService.HubConnection!.InvokeAsync<bool>("GroupConnect",
                $"{ConnectedGroup!.RoomName}", RoomPassword);

            if (connectRoomStatus)
            {
                _mixer.RemoveAllMixerInputs();

                _bufferUsers.Clear();

                CurrentGroup = ConnectedGroup;

                MicrophoneMute = false;

                _waveOut.Play();

                try
                {
                    _input.StartRecording();
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                _statusServices.ChangeStatus(new StatusMessage
                    { Message = "Failed connect to the room", IsError = true });
            }
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Failed connect to the room", IsError = true });
        }
    }


    private async void AsyncGroupDisconect()
    {
        try
        {
            await ServerService.HubConnection!.InvokeAsync("GroupDisconnect");
            GroupDisconnected();
        }
        catch 
        {
            _statusServices.ChangeStatus(new StatusMessage
                { Message = "Failed disconnect from group", IsError = true });
        }
    }


    private void GroupDisconnected()
    {
        _mixer.RemoveAllMixerInputs();

        _bufferUsers.Clear();

        CurrentGroup = null;

        MessagesColCollection = new ObservableCollection<Message>();

        _waveOut.Stop();

        MicrophoneMute = false;

        _mixer.RemoveAllMixerInputs();

        _bufferUsers.Clear();
    }

    private async void AsyncGetUserServerName()
    {
        string? newName = null;
        try
        {
            newName = await ServerService.HubConnection!
                .InvokeAsync<string>("SetName", _serverAccountStore.CurrentAccount!.Login).ConfigureAwait(false);
        }
        catch 
        {
            Dispose();
        }

        if (newName == "non") Dispose();

        _serverAccountStore.CurrentAccount!.CurrentServerLogin = newName;
    }

    #endregion

    #region Audio

    private async void InputDataAvailable(object sender, WaveInEventArgs e)
    {
        try
        {
            if (CurrentGroup != null)
            {
                if (VAD(e))
                    await ServerService.HubConnection!.SendAsync("SendAudio", e.Buffer);
            }
            else
            {
                MicrophoneMute = true;
            }
        }
        catch
        {
            // ignored
        }
    }

    private bool VAD(WaveInEventArgs e)
    {
        var porog = 0.005;

        var tr = false;

        double sum2 = 0;

        var count = e.BytesRecorded / 2;


        for (var index = 0; index < e.BytesRecorded; index += 2)
        {
            double Tmp = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);

            Tmp /= 32768.0;

            sum2 += Tmp * Tmp;

            if (Tmp > porog)

                tr = true;
        }

        sum2 /= count;

        return tr || sum2 > porog;
    }

    public async void AsyncGetAudioBus(VoiceMessage newVoiceMes)
    {
        if (HeadphoneMute) return;

        try
        {
            var bufferUser = _bufferUsers[newVoiceMes.Name!];

            bufferUser.AddSamples(newVoiceMes.Audio, 0, newVoiceMes.Audio!.Length);
        }
        catch 
        {
            try
            {
                _bufferUsers.Add(newVoiceMes.Name!, new BufferedWaveProvider(_waveFormat));

                var bufferUser = _bufferUsers[newVoiceMes.Name!];

                _mixer.AddMixerInput(bufferUser);
            }
            catch
            {
                // ignored
            }
        }
        finally
        {
            if (_waveOut.PlaybackState == PlaybackState.Stopped) _waveOut.Play();
        }
    }

    #endregion

    #region Subscriptions

    private void AsyncGetMessageBus(Message newMessage)
    {
        MessagesColCollection!.Add(newMessage);
    }

    private async void AsynGetServerRoomsBus()
    {
        try
        {
            var roomListFromServer =
                await ServerService.HubConnection!.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");


            ServerRooms = new ObservableCollection<ServerRooms>(roomListFromServer);
        }
        catch 
        {
            ServerRooms = new ObservableCollection<ServerRooms>();
        }
    }

    #endregion

    #region Changed

    private ObservableCollection<ServerRooms>? OnCurrentGoupChanged(ObservableCollection<ServerRooms>? CurrentRoomUsers)
    {
        foreach (var rooms in CurrentRoomUsers!)
        foreach (var users in rooms.Users!)
            if (users.Login == _serverAccountStore.CurrentAccount!.CurrentServerLogin)
                users.Role = "You";


        return CurrentRoomUsers;
    }


    private void OnMicrophoneMuteChanged()
    {
        if (MicrophoneMute)
            try
            {
                _input.StopRecording();
            }
            catch
            {
                // ignored
            }
        else
            try
            {
                _input.StartRecording();
            }
            catch
            {
                // ignored
            }
    }

    #endregion

    public override void Dispose()
    {
        ServerService.HubConnection!.StopAsync();

        MessageBus.Bus -= AsyncGetMessageBus;

        ReloadServerRooms.Reloader -= AsynGetServerRoomsBus;

        VoiceMessageBus.Bus -= AsyncGetAudioBus;

        _input.StopRecording();

        _waveOut.Stop();

        CurrentServerStore.Delete();

        _serverAccountStore.Logout();

        base.Dispose();
    }

    #endregion


    #region Data

    private readonly Dictionary<string, BufferedWaveProvider> _bufferUsers = new();

    private ObservableCollection<Message>? _messagesColCollection;

    public ObservableCollection<Message>? MessagesColCollection
    {
        get => _messagesColCollection;
        set => Set(ref _messagesColCollection, value);
    }


    private string? _currentMessage;

    public string? CurrentMessage
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


    private string? _newRoomName;

    public string? NewRoomName
    {
        get => _newRoomName;
        set => Set(ref _newRoomName, value);
    }


    private string? _newRoomPassword;

    public string? NewRoomPassword
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


    private string? _roomPassword;

    public string? RoomPassword
    {
        get => _roomPassword;
        set => Set(ref _roomPassword, value);
    }

    private ServerRooms? _selRooms;

    public ServerRooms? SelRooms
    {
        get => _selRooms;
        set => Set(ref _selRooms, value);
    }


    private ServerUser? _selServerUser;

    public ServerUser? SelServerUser
    {
        get => _selServerUser;
        set => Set(ref _selServerUser, value);
    }


    private ObservableCollection<ServerRooms>? _serverRooms;

    public ObservableCollection<ServerRooms>? ServerRooms
    {
        get => _serverRooms;
        set => Set(ref _serverRooms, OnCurrentGoupChanged(value));
    }


    private ServerRooms? _currentGroup;

    public ServerRooms? CurrentGroup
    {
        get => _currentGroup;
        set => Set(ref _currentGroup, value);
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


    private bool _microphoneMute;

    public bool MicrophoneMute
    {
        get => _microphoneMute;
        set
        {
            Set(ref _microphoneMute, value);
            OnMicrophoneMuteChanged();
        }
    }

    #endregion
}