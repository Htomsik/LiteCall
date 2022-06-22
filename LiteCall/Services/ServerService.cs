
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Infrastructure.Bus;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace SignalRServ
{
    internal class ServerService
    {
        public static HubConnection hubConnection;


        private static bool _isRecconectingDisconnect;

        public static Task ConnectionHub(string url, Account currentAccount, IStatusServices statusServices)
        {

            statusServices.ChangeStatus(new StatusMessage{Message = "Connecting to server. . .", isError = false});

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{url}?token={currentAccount.Token}", options =>
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
                    TimeSpan.Zero,TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(10)
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
                statusServices.ChangeStatus(new StatusMessage{isError = true,Message = "You have been kicked from room"});

                DisconnectNotification.Reload();

                
            });

            hubConnection.On("UpdateRooms", () =>
            {

                ReloadServerRooms.Reload();

                return Task.CompletedTask;

            });

            hubConnection.On("SendAudio", (string name, byte[] MessageAudio) =>
            {
                VoiceMessage newMessage = new VoiceMessage {Name = name, AudioByteArray = MessageAudio};

                VoiceMessageBus.Send(newMessage);

            });


            //если соединение закрыто
            hubConnection.Closed += error =>
            {

                if (_isRecconectingDisconnect)
                {
                    statusServices.ChangeStatus(new StatusMessage
                        { isError = true, Message = "Reconnecting failed" });
                }
                else
                {
                    statusServices.DeleteStatus();
                }

                DisconectServerReloader.Reload();

                _isRecconectingDisconnect = false;

                return Task.CompletedTask;
            };

     

            //возникает когда получается обратно подключится
            hubConnection.Reconnected += id =>
            {
                statusServices.DeleteStatus();

                _isRecconectingDisconnect = false;

                return Task.CompletedTask;
            };


         

            //возникает в момент переподключения
            hubConnection.Reconnecting += error =>
            {
                _isRecconectingDisconnect = true;

                statusServices.ChangeStatus(new StatusMessage { Message = $"Reconecting to server. . ." });

                return Task.CompletedTask;
            };

            return hubConnection.StartAsync();

        }



        

        
    }
}
