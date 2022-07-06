using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Infrastructure.Buses;
using Core.Infrastructure.CMD.Lambda;
using Core.Infrastructure.Notifiers;
using Core.Models.Servers;
using Core.Models.Servers.Messages;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using LiteCall.Services.Interfaces;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LiteCall.ViewModels.ServerPages;

internal sealed class ServerVmd : BaseVmd
{
    public ServerVmd(CurrentServerAccountStore currentServerAccountStore, CurrentServerStore currentServerStore,
        IStatusSc statusSc, IChatServerSc chatServerSc)
    {
        _currentServerAccountStore = currentServerAccountStore;

        MessagesColCollection = new ObservableCollection<TextMessage>();

        CurrentServerStore = currentServerStore;

        _statusSc = statusSc;

        _chatServerSc = chatServerSc;

        currentServerStore.CurrentServerDeleted += Dispose;


        TextMessageBus.Bus += AsyncGetMessageBus;

        AudioMessageBus.Bus += AsyncGetAudioBus;

        KickFromRoomNotifier.Notificator += GroupDisconnected;

        CurrentServerStore.CurrentServerRoomsChanged += CurrentServerRoomsChanged;

        _currentServerAccountStore.CurrentAccountChange += CurrentAccountChange;


        #region команды

        SendMessageCommand = new AsyncLambdaCmd(OnSendMessageExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanSendMessageExecuted);

        CreateNewRoomCommand = new AsyncLambdaCmd(OnCreateNewRoomExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanCreateNewRoomExecute);

        ConnectCommand = new AsyncLambdaCmd(OnConnectExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanConnectExecute);

        ConnectWithPasswordCommand = new AsyncLambdaCmd(OnConnectWithPasswordCommandExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanConnectWithPasswordExecute);

        OpenCreateRoomModalCommand = new LambdaCmd(OnOpenCreateRoomModalCommandExecuted);

        OpenPasswordModalCommand = new LambdaCmd(OnOpenPasswordModalCommandCommandExecuted);

        DisconnectGroupCommand = new AsyncLambdaCmd(OnDisconnectGroupExecuted,
            ex => statusSc.ChangeStatus(ex.Message));


        #region Админ команды

        AdminDeleteRoomCommand = new AsyncLambdaCmd(OnAdminDeleteRoomExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanAdminDeleteRoomExecute);

        AdminDisconnectUserFromRoomCommand = new AsyncLambdaCmd(OnAdminKickUserFromRoomExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanAdminDisconnectUserFromRoomExecute);

        #endregion

        #endregion

        #region Naudio Settings

        _input = new WaveIn();

        _input.DataAvailable += InputDataAvailable!;

        _input.BufferMilliseconds = 25;

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

    private void CurrentAccountChange()
    {
        OnPropertyChanged(nameof(CanServerConnect));
    }

    private void CurrentServerRoomsChanged()
    {
        OnPropertyChanged(nameof(CurrentGroup));
    }

    #region Services

    private readonly IStatusSc _statusSc;

    private readonly IChatServerSc _chatServerSc;

    #endregion

    #region Stores

    public CurrentServerStore CurrentServerStore { get; set; }

    private readonly CurrentServerAccountStore _currentServerAccountStore;

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

    private async Task OnDisconnectGroupExecuted(object p)
    {
        await _chatServerSc.GroupDisconnect();
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
        await _chatServerSc.GroupCreate(NewRoomName!, NewRoomPassword!);

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
        var newMessage = new TextMessage
        {
            DateSend = DateTime.Now,
            Text = CurrentMessage,
            Sender = _currentServerAccountStore.CurrentAccount!.CurrentServerLogin
        };

        if (await _chatServerSc.SendMessage(newMessage))
        {
            MessagesColCollection!.Add(newMessage);
            CurrentMessage = string.Empty;
        }
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
            return !string.Equals(ConnectedGroup.RoomName!, CurrentGroup.RoomName!,
                StringComparison.CurrentCultureIgnoreCase);
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

        await _chatServerSc.GroupConnect(SelRooms!.RoomName!, RoomPassword!);
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
        await _chatServerSc.GroupConnect(SelRooms!.RoomName!, RoomPassword!);
        RoomPassword = string.Empty;
        RoomPasswordModalStatus = false;
    }

    #endregion

    #region AdminDeleteRoom

    public ICommand AdminDeleteRoomCommand { get; }

    private bool CanAdminDeleteRoomExecute(object p)
    {
        if (_currentServerAccountStore.CurrentAccount!.Role != "Admin") return false;

        return p is ServerRooms;
    }

    private async Task OnAdminDeleteRoomExecuted(object p)
    {
        var deletedRoom = (ServerRooms)p;

        await _chatServerSc.AdminDeleteGroup(deletedRoom.RoomName!);
    }

    #endregion

    #region AdminKickUserFromRoom

    public ICommand AdminDisconnectUserFromRoomCommand { get; }

    private bool CanAdminDisconnectUserFromRoomExecute(object p)
    {
        if (_currentServerAccountStore.CurrentAccount!.Role != "Admin") return false;

        return p is ServerUser;
    }

    private async Task OnAdminKickUserFromRoomExecuted(object p)
    {
        var kickedUser = (ServerUser)p;

        await _chatServerSc.AdminKickUserFromGroup(kickedUser.Login!);
    }

    #endregion

    #endregion

    #region Methods

    #region ControlMethods

    //private async Task AsyncRoomConnect(ServerRooms? ConnectedGroup, string? RoomPassword = "")
    //{
    //    try
    //    {
    //        var connectRoomStatus = await ServerService.HubConnection!.InvokeAsync<bool>("GroupConnect",
    //            $"{ConnectedGroup!.RoomName}", RoomPassword);

    //        if (connectRoomStatus)
    //        {
    //            _mixer.RemoveAllMixerInputs();

    //            _bufferUsers.Clear();

    //            CurrentGroup = ConnectedGroup;

    //            MicrophoneMute = false;

    //            _waveOut.Play();

    //            try
    //            {
    //                _input.StartRecording();
    //            }
    //            catch
    //            {
    //                // ignored
    //            }
    //        }
    //        else
    //        {
    //            _statusServices.ChangeStatus(new StatusMessage
    //                { Message = "Failed connect to the room", IsError = true });
    //        }
    //    }
    //    catch
    //    {
    //        _statusServices.ChangeStatus(new StatusMessage { Message = "Failed connect to the room", IsError = true });
    //    }
    //}


    private void GroupDisconnected()
    {
        _mixer.RemoveAllMixerInputs();

        _bufferUsers.Clear();

        MessagesColCollection = new ObservableCollection<TextMessage>();

        _waveOut.Stop();

        MicrophoneMute = false;

        _mixer.RemoveAllMixerInputs();

        _bufferUsers.Clear();
    }

    #endregion

    #region Audio

    private async void InputDataAvailable(object sender, WaveInEventArgs e)
    {
        if (CurrentGroup != null)
        {
            if (VAD(e))
                await _chatServerSc.SendAudioMessage(e.Buffer);
        }
        else
        {
            MicrophoneMute = true;
        }
    }

    private bool VAD(WaveInEventArgs e)
    {
        const double porog = 0.005;

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

    public void AsyncGetAudioBus(AudioMessage newVoiceMes)
    {
        if (HeadphoneMute) return;

        try
        {
            var bufferUser = _bufferUsers[newVoiceMes.UserName!];

            bufferUser.AddSamples(newVoiceMes.Audio, 0, newVoiceMes.Audio!.Length);
        }
        catch
        {
            try
            {
                _bufferUsers.Add(newVoiceMes.UserName!, new BufferedWaveProvider(_waveFormat));

                var bufferUser = _bufferUsers[newVoiceMes.UserName!];

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

    private void AsyncGetMessageBus(TextMessage newMessage)
    {
        MessagesColCollection!.Add(newMessage);
    }

    #endregion

    #region Changed

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
        _chatServerSc.ConnectionStop();

        TextMessageBus.Bus -= AsyncGetMessageBus;

        AudioMessageBus.Bus -= AsyncGetAudioBus;

        _input.StopRecording();

        _waveOut.Stop();
        
        _currentServerAccountStore.Logout();

        base.Dispose();
    }

    #endregion

    #region Data

    private readonly Dictionary<string, BufferedWaveProvider> _bufferUsers = new();

    private ObservableCollection<TextMessage>? _messagesColCollection;

    public ObservableCollection<TextMessage>? MessagesColCollection
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


    public ServerRooms? CurrentGroup => SetCurrentGroup();

    private ServerRooms SetCurrentGroup()
    {
        return (from rooms in CurrentServerStore.CurrentServerRooms!
            from users in rooms.Users!
            where users.Login == _currentServerAccountStore.CurrentAccount!.CurrentServerLogin
            select rooms).FirstOrDefault()!;
    }


    public bool CanServerConnect => true;


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