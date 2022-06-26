using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Bus;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Model.Errors;
using LiteCall.Model.ServerModels;
using LiteCall.Model.ServerModels.Messages;
using LiteCall.Model.Users;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LiteCall.ViewModels.ServerPages;

internal sealed class ServerVmd : BaseVmd
{
    #region Services

    private readonly IStatusServices _statusServices;

    private readonly IChatServerServices _chatServerServices;

    #endregion

    public ServerVmd(ServerAccountStore serverAccountStore, CurrentServerStore currentServerStore,
        IStatusServices statusServices, IChatServerServices chatServerServices)
    {
        _serverAccountStore = serverAccountStore;

        MessagesColCollection = new ObservableCollection<Message>();

        CurrentServerStore = currentServerStore;

        _statusServices = statusServices;

        _chatServerServices = chatServerServices;

        currentServerStore.CurrentServerDeleted += Dispose;


        MessageBus.Bus += AsyncGetMessageBus;

        VoiceMessageBus.Bus += AsyncGetAudioBus;

        DisconnectNotification.Notificator += GroupDisconnected;

        CurrentServerStore.CurrentServerRoomsChanged += CurrentServerRoomsChanged;

        _serverAccountStore.CurrentAccountChange += CurrentAccountChange;



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

        DisconnectGroupCommand = new AsyncLambdaCommand(OnDisconnectGroupExecuted, ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }));


        #region Админ команды

        AdminDeleteRoomCommand = new AsyncLambdaCommand(OnAdminDeleteRoomExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanAdminDeleteRoomExecute);

        AdminDisconnectUserFromRoomCommand = new AsyncLambdaCommand(OnAdminKickUserFromRoomExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
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

    #region Stores

    public CurrentServerStore CurrentServerStore { get; set; }

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

    private async Task OnDisconnectGroupExecuted(object p)
    {
       await _chatServerServices.GroupDisconnect();
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
        await _chatServerServices.GroupCreate(NewRoomName!, NewRoomPassword!);

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

        if (await _chatServerServices.SendMessage(newMessage))
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

        await _chatServerServices.GroupConnect(SelRooms!.RoomName!,RoomPassword!);
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
        await _chatServerServices.GroupConnect(SelRooms!.RoomName!, RoomPassword!);
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

        await _chatServerServices.AdminDeleteGroup(deletedRoom.RoomName!);
    }

    #endregion

    #region AdminKickUserFromRoom

    public ICommand AdminDisconnectUserFromRoomCommand { get; }

    private bool CanAdminDisconnectUserFromRoomExecute(object p)
    {
        if (_serverAccountStore.CurrentAccount!.Role != "Admin") return false;

        return p is ServerUser;
    }

    private async Task OnAdminKickUserFromRoomExecuted(object p)
    {
        var kickedUser = (ServerUser)p;

        await _chatServerServices.AdminKickUserFromGroup(kickedUser.Login!);
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

        MessagesColCollection = new ObservableCollection<Message>();

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
                    await _chatServerServices.SendAudioMessage(e.Buffer);
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

    public void AsyncGetAudioBus(VoiceMessage newVoiceMes)
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
        _chatServerServices.ConnectionStop();

        MessageBus.Bus -= AsyncGetMessageBus;

       // ReloadServerRooms.Reloader -= AsynGetServerRoomsBus;

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


    
    public ServerRooms? CurrentGroup => SetCurrentGroup();

    private ServerRooms SetCurrentGroup()
    {
        return (from rooms in CurrentServerStore.CurrentServerRooms! from users in rooms.Users! where users.Login == _serverAccountStore.CurrentAccount!.CurrentServerLogin select rooms).FirstOrDefault()!;
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