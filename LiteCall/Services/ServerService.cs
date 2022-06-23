using System;
using System.Net.Http;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Bus;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace LiteCall.Services;

internal class ServerService
{
    public static  HubConnection? HubConnection;


    private static bool _isReconnectingDisconnect;

    public static Task ConnectionHub(string url, Account? currentAccount, IStatusServices statusServices)
    {
        statusServices.ChangeStatus(new StatusMessage { Message = "Connecting to server. . .", IsError = false });

        HubConnection = new HubConnectionBuilder()
            .WithUrl($"{url}?token={currentAccount!.Token}", options =>
            {
                options.WebSocketConfiguration = conf =>
                {
                    conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
                };
                options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                options.AccessTokenProvider = () => Task.FromResult(currentAccount.Token);
            })
            .WithAutomaticReconnect(new[]
            {
                TimeSpan.Zero, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)
            })
            .Build();

        HubConnection.ServerTimeout = TimeSpan.FromSeconds(10000);

        HubConnection.On<Message>("Send", message =>
        {
            MessageBus.Send(message);
            return Task.CompletedTask;
        });

        HubConnection.On<bool>("Notification", flag =>
        {
            statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "You have been kicked from room" });

            DisconnectNotification.Reload();
        });

        HubConnection.On("UpdateRooms", () =>
        {
            ReloadServerRooms.Reload();

            return Task.CompletedTask;
        });

        HubConnection.On("SendAudio", (string name, byte[] MessageAudio) =>
        {
            var newMessage = new VoiceMessage { Name = name, Audio = MessageAudio };

            VoiceMessageBus.Send(newMessage);
        });


        //если соединение закрыто
        HubConnection.Closed += error =>
        {
            if (_isReconnectingDisconnect)
                statusServices.ChangeStatus(new StatusMessage
                    { IsError = true, Message = "Reconnecting failed" });
            else
                statusServices.DeleteStatus();

            DisconnectServerReloader.Reload();

            _isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };


        //возникает когда получается обратно подключится
        HubConnection.Reconnected += id =>
        {
            statusServices.DeleteStatus();

            _isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };


        //возникает в момент переподключения
        HubConnection.Reconnecting += error =>
        {
            _isReconnectingDisconnect = true;

            statusServices.ChangeStatus(new StatusMessage { Message = "Reconecting to server. . ." });

            return Task.CompletedTask;
        };

        return HubConnection.StartAsync();
    }
}