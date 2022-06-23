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
    
    public static Task ConnectionHub(string url, Account? currentAccount, IStatusServices statusServices)
    {

         bool isReconnectingDisconnect = false;

        statusServices.ChangeStatus(new StatusMessage { Message = "Connecting to server. . .", IsError = false });

       var hubConnection = new HubConnectionBuilder()
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

        hubConnection.ServerTimeout = TimeSpan.FromSeconds(10000);

        hubConnection.On<Message>("Send", message =>
        {
            MessageBus.Send(message);
            return Task.CompletedTask;
        });

        hubConnection.On<bool>("Notification", flag =>
        {
            statusServices.ChangeStatus(
                new StatusMessage { IsError = true, Message = "You have been kicked from room" });

            DisconnectNotification.Reload();
        });

        hubConnection.On("UpdateRooms", () =>
        {
            ReloadServerRooms.Reload();

            return Task.CompletedTask;
        });

        hubConnection.On("SendAudio", (string name, byte[] MessageAudio) =>
        {
            var newMessage = new VoiceMessage { Name = name, Audio = MessageAudio };

            VoiceMessageBus.Send(newMessage);
        });


        //если соединение закрыто
        hubConnection.Closed += error =>
        {
            if (isReconnectingDisconnect)
                statusServices.ChangeStatus(new StatusMessage
                    { IsError = true, Message = "Reconnecting failed" });
            else
                statusServices.DeleteStatus();

            DisconnectServerReloader.Reload();

            isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };


        //возникает когда получается обратно подключится
        hubConnection.Reconnected += id =>
        {
            statusServices.DeleteStatus();

            isReconnectingDisconnect = false;

            return Task.CompletedTask;
        };


        //возникает в момент переподключения
        hubConnection.Reconnecting += error =>
        {
            isReconnectingDisconnect = true;

            statusServices.ChangeStatus(new StatusMessage { Message = "Reconecting to server. . ." });

            return Task.CompletedTask;
        };

        return hubConnection.StartAsync();
    }
}