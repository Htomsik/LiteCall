using System.Collections.ObjectModel;
using Core.Infrastructure.Buses;
using Core.Infrastructure.Notifiers;
using Core.Models.AppInfrastructure;
using Core.Models.AppInfrastructure.StateStatuses;
using Core.Models.Servers;
using Core.Models.Servers.Messages;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.Connections;
using Core.Stores.TemporaryInfo;
using Microsoft.AspNetCore.SignalR.Client;

namespace Core.Services.Connections;

public sealed class ChatServerSc : IChatServerSc
{
    private readonly CurrentServerStore _currentServerStore;

    private readonly HubConnectionStore? _hubConnectionStore;

    private readonly CurrentServerAccountStore _currentServerAccountStore;

    private readonly IStatusSc _statusSc;


    public ChatServerSc(IStatusSc statusSc, HubConnectionStore hubConnectionStore,
        CurrentServerAccountStore currentServerAccountStore, CurrentServerStore currentServerStore)
    {
        _statusSc = statusSc;

        _hubConnectionStore = hubConnectionStore;

        _currentServerAccountStore = currentServerAccountStore;

        _currentServerStore = currentServerStore;

        _statusSc.ChangeStatus(ExecutionActionStates.ServerConnection);

        ConnectionStart();
        
        GetUserServerName();

        _statusSc.DeleteStatus();
    }

    public async Task<bool> GroupConnect(string roomName, string roomPassword)
    {
        try
        {
            return await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync<bool>("GroupConnect",
                $"{roomName}", roomPassword);
        }
        catch
        {
            _statusSc.ChangeStatus(
                new AppExecutionState { Type = StateTypes.Error, Message = "Room connection error" });

            return false;
        }
    }

    public async Task<bool> GroupDisconnect()
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync("GroupDisconnect");
            return true;
        }
        catch
        {
            _statusSc.ChangeStatus(
                new AppExecutionState { Type = StateTypes.Error, Message = "Room disconnect error" });

            return false;
        }
    }

    public async Task<bool> GroupCreate(string roomName, string roomPassword)
    {
        try
        {
            return await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync<bool>("GroupCreate",
                $"{roomName}", roomPassword);
        }
        catch
        {
            _statusSc.ChangeStatus(
                new AppExecutionState { Type = StateTypes.Error, Message = "Room creation error" });

            return false;
        }
    }
    

    public async Task<bool> AdminDeleteGroup(string roomName)
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.SendAsync("AdminDeleteRoom", roomName);
            return true;
        }
        catch
        {
            _statusSc.ChangeStatus(
                new AppExecutionState { Type = StateTypes.Error, Message = "Room delete error" });
            return false;
        }
    }

    public async Task<bool> AdminKickUserFromGroup(string userName)
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.SendAsync("AdminKickUser", userName);
            return true;
        }
        catch
        {
            _statusSc.ChangeStatus(
                new AppExecutionState { Type = StateTypes.Error, Message = "User kick error" });
            return false;
        }
    }

    public async Task<bool> SendMessage(TextMessage newMessage)
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync("SendMessage", newMessage);

            return true;
        }
        catch
        {
            _statusSc.ChangeStatus(new AppExecutionState
                { Message = "Failed send message", Type = StateTypes.Error });

            return false;
        }
    }

    public async Task ConnectionStop()
    {
        await _hubConnectionStore!.StopConnection();

        await _currentServerStore.Delete();
    }

    public async Task SendAudioMessage(byte[] audioBuffer)
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync("SendAudio", audioBuffer);
        }
        catch
        {
            _statusSc.ChangeStatus(new AppExecutionState { Message = "Failed send Audio", Type = StateTypes.Error });
        }
    }

    public async Task GetUserServerName()
    {
        var newName = "";
        try
        {
            newName = await _hubConnectionStore!.CurrentHubConnection!
                .InvokeAsync<string>("SetName", _currentServerAccountStore.CurrentAccount!.Login).ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(new AppExecutionState
                { Message = "Failed get current account login", Type = StateTypes.Error });

            await _currentServerStore.Delete();
        }

        if (newName == "non")
            await _currentServerStore.Delete();
        else
            _currentServerAccountStore.CurrentAccount!.CurrentServerLogin = newName;
    }

    private async Task GetServerRooms()
    {
        try
        {
            var roomListFromServer =
                await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");

            _currentServerStore.CurrentServerRooms =
                new ObservableCollection<ServerRooms>(OnGroupCollectionChanged(roomListFromServer)!);
        }
        catch
        {
            _currentServerStore.CurrentServerRooms = new ObservableCollection<ServerRooms>();
        }
    }

    private List<ServerRooms> OnGroupCollectionChanged(List<ServerRooms>? currentRoomUsers)
    {
        for (var index = 0; index < currentRoomUsers!.Count; index++)
        {
            var rooms = currentRoomUsers![index];
            foreach (var users in rooms.Users!)
                if (users.Login == _currentServerAccountStore.CurrentAccount!.CurrentServerLogin)
                    users.Role = "You";
        }


        return currentRoomUsers;
    }

    private  Task ConnectionStart()
    {
        var isReconnectingDisconnect = false;

        _hubConnectionStore!.CurrentHubConnection = new HubConnectionBuilder()
            .WithUrl(
                $"https://{_currentServerStore.CurrentServer!.Ip}/LiteCall?token={_currentServerAccountStore.CurrentAccount!.Token}",
                options =>
                {
                    options.WebSocketConfiguration = conf =>
                    {
                        conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => true;
                    };
                    options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    options.AccessTokenProvider = () => Task.FromResult(_currentServerAccountStore.CurrentAccount.Token);
                })
            .WithAutomaticReconnect(new[]
            {
                TimeSpan.Zero, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)
            })
            
            .Build();

        _hubConnectionStore.CurrentHubConnection.On<TextMessage>("Send", message =>
        {
            TextMessageBus.Send(message);
            return Task.CompletedTask;
        });

        _hubConnectionStore.CurrentHubConnection.On<bool>("Notification", flag =>
        {
            _statusSc.ChangeStatus(
                new AppExecutionState { Type = StateTypes.Error, Message = "You have been kicked from room" });

            KickFromRoomNotifier.Notify();
        });

        _hubConnectionStore.CurrentHubConnection.On("UpdateRooms", async () =>
        {
          await  GetServerRooms();
          
        });

        _hubConnectionStore.CurrentHubConnection.On("SendAudio", (string name, byte[] messageAudio) =>
        {
            var newMessage = new AudioMessage { UserName = name, Audio = messageAudio };

            AudioMessageBus.Send(newMessage);
        });

        _hubConnectionStore.CurrentHubConnection.Closed += error =>
        {
            if (isReconnectingDisconnect)
                _statusSc.ChangeStatus(new AppExecutionState
                    { Type = StateTypes.Error, Message = "Reconnecting failed" });
            else
                _statusSc.DeleteStatus();

            _currentServerStore.Delete();

            isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };

        _hubConnectionStore.CurrentHubConnection.Reconnected += id =>
        {
            _statusSc.DeleteStatus();

            isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };

        _hubConnectionStore.CurrentHubConnection.Reconnecting += error =>
        {
            isReconnectingDisconnect = true;

            _statusSc.ChangeStatus(new AppExecutionState
                { Message = "Reconnecting to server. . .", Type = StateTypes.Action });

            return Task.CompletedTask;
        };
        
      

        return _hubConnectionStore.CurrentHubConnection.StartAsync();
    }
}