using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Bus;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using Microsoft.AspNetCore.SignalR.Client;

namespace LiteCall.Services;

internal sealed class ChatServerServices : IChatServerServices
{
    private readonly CurrentServerStore _currentServerStore;

    private readonly HubConnectionStore? _hubConnectionStore;

    private readonly ServerAccountStore _serverAccountStore;

    private readonly IStatusServices _statusServices;


    public ChatServerServices(IStatusServices statusServices, HubConnectionStore hubConnectionStore,
        ServerAccountStore serverAccountStore, CurrentServerStore currentServerStore)
    {

       

        _statusServices = statusServices;

        _hubConnectionStore = hubConnectionStore;

        _serverAccountStore = serverAccountStore;

        _currentServerStore = currentServerStore;

        _statusServices!.ChangeStatus(new StatusMessage { Message = "Connecting to server. . .", IsError = false });

        ConnectionStart();

        GetUserServerName();

        _statusServices.DeleteStatus();
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
            _statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "Room connection error" });

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
            _statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "Room disconnect error" });

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
            _statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "Room creation error" });

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

            _statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "Room delete error" });
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
            _statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "User kick error" });
            return false;
        }
    }

    public async Task<bool> SendMessage(Message newMessage)
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync("SendMessage", newMessage);

            return true;
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Failed send message", IsError = true });

            return false;
        }
    }

    public async Task ConnectionStop()
    {
        await _hubConnectionStore!.StopConnection();

         _currentServerStore.Delete();
    }

    public async Task SendAudioMessage(byte[] audioBuffer)
    {
        try
        {
            await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync("SendAudio", audioBuffer);

        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Failed send Audio", IsError = true });

        }
    }

    private async Task GetUserServerName()
    {
        string newName = "";
        try
        {
            newName = await _hubConnectionStore!.CurrentHubConnection!
                        .InvokeAsync<string>("SetName", _serverAccountStore.CurrentAccount!.Login).ConfigureAwait(false);
        }
        catch 
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Failed get current account login", IsError = true });

            _currentServerStore.Delete();
        }

        if (newName == "non")
        {
            _currentServerStore.Delete();
        }
        else
        {
            _serverAccountStore.CurrentAccount!.CurrentServerLogin = newName;
        
        }
    }

    private async Task GetServerRooms()
    {
        try
        {
            var roomListFromServer =
                await _hubConnectionStore!.CurrentHubConnection!.InvokeAsync<List<ServerRooms>>("GetRoomsAndUsers");

            _currentServerStore.CurrentServerRooms = new ObservableCollection<ServerRooms>(OnGroupCollectionChanged(roomListFromServer)!);
        }
        catch
        {
            _currentServerStore.CurrentServerRooms = new ObservableCollection<ServerRooms>();
        }
    }

    private List<ServerRooms>? OnGroupCollectionChanged(List<ServerRooms>? currentRoomUsers)
    {
        for (var index = 0; index < currentRoomUsers!.Count; index++)
        {
            var rooms = currentRoomUsers![index];
            foreach (var users in rooms.Users!)
                if (users.Login == _serverAccountStore.CurrentAccount!.CurrentServerLogin)
                    users.Role = "You";
        }


        return currentRoomUsers;
    }

    private Task ConnectionStart()
    {
        var isReconnectingDisconnect = false;

     

        _hubConnectionStore!.CurrentHubConnection = new HubConnectionBuilder()
            .WithUrl(
                $"https://{_currentServerStore.CurrentServer!.Ip}/LiteCall?token={_serverAccountStore.CurrentAccount!.Token}",
                options =>
                {
                    options.WebSocketConfiguration = conf =>
                    {
                        conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) =>
                        {
                            return true;
                        };
                    };
                    options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
                    options.AccessTokenProvider = () => Task.FromResult(_serverAccountStore.CurrentAccount.Token);
                })
            .WithAutomaticReconnect(new[]
            {
                TimeSpan.Zero, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)
            })
            .Build();

        _hubConnectionStore.CurrentHubConnection.ServerTimeout = TimeSpan.FromSeconds(10000);

        _hubConnectionStore.CurrentHubConnection.On<Message>("Send", message =>
        {
            MessageBus.Send(message);
            return Task.CompletedTask;
        });

        _hubConnectionStore.CurrentHubConnection.On<bool>("Notification", flag =>
        {
            _statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "You have been kicked from room" });

            DisconnectNotification.Reload();
        });

        _hubConnectionStore.CurrentHubConnection.On("UpdateRooms", () =>
        {
            GetServerRooms();

            return Task.CompletedTask;
        });

        _hubConnectionStore.CurrentHubConnection.On("SendAudio", (string name, byte[] MessageAudio) =>
        {
            var newMessage = new VoiceMessage { Name = name, Audio = MessageAudio };

            VoiceMessageBus.Send(newMessage);
        });

        _hubConnectionStore.CurrentHubConnection.Closed += error =>
        {
            if (isReconnectingDisconnect)
                _statusServices.ChangeStatus(new StatusMessage
                { IsError = true, Message = "Reconnecting failed" });
            else
                _statusServices.DeleteStatus();

            DisconnectServerReloader.Reload();

            isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };

        _hubConnectionStore.CurrentHubConnection.Reconnected += id =>
        {
            _statusServices.DeleteStatus();

            isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };

        _hubConnectionStore.CurrentHubConnection.Reconnecting += error =>
        {
            isReconnectingDisconnect = true;

            _statusServices.ChangeStatus(new StatusMessage { Message = "Reconecting to server. . ." });

            return Task.CompletedTask;
        };

        return _hubConnectionStore.CurrentHubConnection.StartAsync();
    }
}