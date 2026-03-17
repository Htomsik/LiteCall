using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows.Input;
using Core.Infrastructure.Buses;
using Core.Infrastructure.CMD.Lambda;
using Core.Infrastructure.Notifiers;
using Core.Models.Servers;
using Core.Models.Servers.Messages;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Audio;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.ServerPages;

public sealed class ServerVmd : BaseVmd
{
    #region Data
    
    private ObservableCollection<TextMessage>? _messagesColCollection;

    public ObservableCollection<TextMessage>? MessagesColCollection
    {
        get => _messagesColCollection ??= new ObservableCollection<TextMessage>();
        set => this.RaiseAndSetIfChanged(ref _messagesColCollection, value);
    }
    
    [Reactive]
    public string? CurrentMessage { get; set; }

    [Reactive]
    public bool CreateRoomModalStatus { get; set; }

    [Reactive]
    public string? NewRoomName { get; set; }

    [Reactive]
    public string? NewRoomPassword { get; set; }

    [Reactive]
    public bool RoomPasswordModalStatus { get; set; }

    [Reactive]
    public string? RoomPassword { get; set; }
    
    /// <summary>
    ///  Selected  Room
    /// </summary>
    [Reactive]
    public ServerRooms? SelRooms {get; set;}
    
    [Reactive] 
    public bool CanServerConnect { get; set; }
    
    [Reactive] 
    public bool HeadphoneMute { get; set; }
    
    [Reactive] 
    public bool MicrophoneMute { get; set; }
    
    public ServerRooms? CurrentGroup => SetCurrentGroup();

    private ServerRooms SetCurrentGroup()
    {
        return (from rooms in CurrentServerStore.CurrentServerRooms!
            from users in rooms.Users!
            where users.Login == CurrentServerAccountStore?.CurrentValue!.CurrentServerLogin
            select rooms).FirstOrDefault()!;
    }
   
    /// <summary>
    ///     Reactive subscribes
    /// </summary>
    private readonly CompositeDisposable _subscribes = new(); 

    #endregion
    
    public ServerVmd(CurrentServerAccountStore currentServerAccountStore, 
        CurrentServerStore currentServerStore,
        IStatusSc statusSc, 
        IChatServerSc chatServerSc,
        IAudioSc audioSc)
    {
        CurrentServerAccountStore = currentServerAccountStore;
        CurrentServerStore = currentServerStore;
        _statusSc = statusSc;
        _chatServerSc = chatServerSc;
        _audioSc = audioSc;

        _audioSc.InputDataGenerated += OnAudioDataGenerated;
        TextMessageBus.Bus += OnGetMessageBus;
        AudioMessageBus.Bus += OnGetAudioBus;
        KickFromRoomNotifier.Notificator += OnGroupDisconnected;

        CurrentServerStore.CurrentServerRoomsChanged += OnCurrentServerRoomsChanged;
        
        #region команды
        
        DisconnectGroupCommand = ReactiveCommand.CreateFromTask(OnDisconnectGroupExecuted);
        OpenPasswordModalCommand = ReactiveCommand.CreateFromTask<object>(OnOpenPasswordModalCommandCommandExecuted);
        OpenCreateRoomModalCommand = ReactiveCommand.CreateFromTask<object>(OnOpenCreateRoomModalCommandExecuted);

        ConnectWithPasswordCommand =
            ReactiveCommand.CreateFromTask<object>(OnConnectWithPasswordCommandExecuted, CanConnectWithPasswordExecute());

        ConnectCommand = ReactiveCommand.CreateFromTask(OnConnectExecuted, CanConnectExecute());
        CreateNewRoomCommand = ReactiveCommand.CreateFromTask(OnCreateNewRoomExecuted,CanCreateNewRoomExecute());
        SendMessageCommand = ReactiveCommand.CreateFromTask(OnSendMessageExecuted,CanSendMessageExecuted());
        
        #region Админ команды

        AdminDeleteRoomCommand = new AsyncLambdaCmd(OnAdminDeleteRoomExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanAdminDeleteRoomExecute);

        AdminDisconnectUserFromRoomCommand = new AsyncLambdaCmd(OnAdminKickUserFromRoomExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanAdminDisconnectUserFromRoomExecute);
        
        #endregion

        #endregion
        
        this.WhenAny(x => x.CurrentServerAccountStore!.CurrentValue!.CurrentServerLogin, x => x.Value)
            .Subscribe(x => OnCurrentServerAccountStoreChanged())
            .DisposeWith(_subscribes);
        
        this.WhenAnyValue(x => x.CurrentGroup)
            .Subscribe(group => { MicrophoneMute = group == null; })
            .DisposeWith(_subscribes);

        this.WhenAnyValue(x => x.MicrophoneMute)
            .Subscribe(_ => OnMicrophoneMuteChanged())
            .DisposeWith(_subscribes);
    }
    
    #region Services

    private readonly IStatusSc _statusSc;

    private readonly IChatServerSc _chatServerSc;
    private readonly IAudioSc _audioSc;

    #endregion

    #region Stores

    public CurrentServerStore CurrentServerStore { get; set; }
    
    private CurrentServerAccountStore? _currentServerAccountStore;

    public CurrentServerAccountStore? CurrentServerAccountStore
    {
        get => _currentServerAccountStore;
        set => this.RaiseAndSetIfChanged(ref _currentServerAccountStore, value);
    }

    #endregion
    
    #region Commands

    #region DiconnecFromGroup

    public IReactiveCommand DisconnectGroupCommand { get; }

    private async Task OnDisconnectGroupExecuted()
    {
        await _chatServerSc.GroupDisconnect();
    }

    #endregion

    #region CreateNewRoom

    public IReactiveCommand CreateNewRoomCommand { get; }

    
    private IObservable<bool> CanCreateNewRoomExecute() => this.WhenAnyValue(x=>x.NewRoomName, 
        newRoomName =>  !string.IsNullOrEmpty(newRoomName));

    private async Task OnCreateNewRoomExecuted()
    {
        await _chatServerSc.GroupCreate(NewRoomName!, NewRoomPassword!);

        NewRoomName = string.Empty;
        NewRoomPassword = string.Empty;
        CreateRoomModalStatus = false;
    }

    #endregion

    #region SendMessage

    public IReactiveCommand SendMessageCommand { get; }
    
    private IObservable<bool> CanSendMessageExecuted() => this.WhenAnyValue(x=>x.CurrentGroup,x=>x.CurrentMessage,
        (currentGroup, currentMessage) => currentGroup is not null && !string.IsNullOrEmpty(currentMessage));
    
    private async Task OnSendMessageExecuted()
    {
        var newMessage = new TextMessage
        {
            DateSend = DateTime.Now,
            Text = CurrentMessage,
            Sender = CurrentServerAccountStore!.CurrentValue!.CurrentServerLogin
        };

        if (await _chatServerSc.SendMessage(newMessage))
        {
            MessagesColCollection!.Add(newMessage);
            CurrentMessage = string.Empty;
        }
    }

    #endregion

    #region OpenCreateRoomModal

    public IReactiveCommand OpenCreateRoomModalCommand { get; }

    private async Task OnOpenCreateRoomModalCommandExecuted(object p)
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

    public IReactiveCommand ConnectCommand { get; }
    
    private IObservable<bool> CanConnectExecute() => this.WhenAnyValue(x => x.CurrentGroup, x=>x.SelRooms,
        (currentGroup,selRooms) =>
        {
            if (selRooms is not ServerRooms rooms) return false;
        
            var ConnectedGroup = rooms;
        
            if (currentGroup is not null)
                return !string.Equals(ConnectedGroup.RoomName!, currentGroup.RoomName!,
                    StringComparison.CurrentCultureIgnoreCase);
            return true;
        });

    private async Task OnConnectExecuted()
    {
        var connectedGroup = SelRooms;
        if (connectedGroup!.WithPassword)
        {
            RoomPasswordModalStatus = true;
            return;
        }

       await _chatServerSc.GroupConnect(SelRooms!.RoomName!, RoomPassword!);
    }

    #endregion

    #region OpenPasswordModal

    public IReactiveCommand OpenPasswordModalCommand { get; }

    private async Task OnOpenPasswordModalCommandCommandExecuted(object p)
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

    public IReactiveCommand ConnectWithPasswordCommand { get; }

    
    private IObservable<bool> CanConnectWithPasswordExecute() => this.WhenAnyValue(x=>x.RoomPassword,
        (roomPassword) =>  !string.IsNullOrEmpty(roomPassword));

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
        if (CurrentServerAccountStore.CurrentValue!.Role != "Admin") return false;

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
        if (CurrentServerAccountStore.CurrentValue!.Role != "Admin") return false;

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
    
    #region Audio

    private async void OnAudioDataGenerated(byte[] audioBuffer)
    {
        if (CurrentGroup != null)
        {
            await _chatServerSc.SendAudioMessage(audioBuffer);
        }
        else
        {
            MicrophoneMute = true;
        }
    }
    
    public void OnGetAudioBus(AudioMessage newVoiceMes)
    {
        if (HeadphoneMute) 
            return;
        
        _audioSc.PlayMessage(newVoiceMes);
    }
    
    private void OnMicrophoneMuteChanged()
    {
        if (MicrophoneMute)
        {
            try { _audioSc.StopRecording(); } catch { /* ignored */ }
        }
        else
        {
            try { _audioSc.StartRecording(); } catch { /* ignored */ }
        }
    }

    #endregion
    
    private void OnCurrentServerRoomsChanged()
    {
        this.RaisePropertyChanged(nameof(CurrentGroup));
    }

    private void OnCurrentServerAccountStoreChanged()
    {
        CanServerConnect = !string.IsNullOrEmpty(CurrentServerAccountStore!.CurrentValue!.CurrentServerLogin);
    }
    
    private void OnGroupDisconnected()
    {
        _audioSc.ClearAudio();
        MessagesColCollection = new ObservableCollection<TextMessage>();
        CurrentMessage = string.Empty;
    }
    
    private void OnGetMessageBus(TextMessage newMessage)
    {
        MessagesColCollection!.Add(newMessage);
    }
    
    public override void Dispose()
    {
        // Stores and Bus
        CurrentServerStore.CurrentServerRoomsChanged -= OnCurrentServerRoomsChanged;
        TextMessageBus.Bus -= OnGetMessageBus;
        AudioMessageBus.Bus -= OnGetAudioBus;
        KickFromRoomNotifier.Notificator -= OnGroupDisconnected;
        
        // Audio
        _audioSc.InputDataGenerated -= OnAudioDataGenerated;
        _audioSc.StopRecording(); 
        _audioSc.ClearAudio();    
        
       // Network 
       _chatServerSc.ConnectionStop();
       
       // Subs
       _subscribes.Dispose();
    }

    #endregion
}