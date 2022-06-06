
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Stores;

namespace SignalRServ
{
    internal class ServerService
    {
        public static HubConnection hubConnection;

    

        public static Task ConnectionHub(string url, Account CurrentAccount)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{url}?token={CurrentAccount.Token}")

                .WithAutomaticReconnect(new[]
                {
                    TimeSpan.Zero, TimeSpan.Zero,
                    TimeSpan.Zero, TimeSpan.FromSeconds(5)
                })

                .Build();

            hubConnection.ServerTimeout = TimeSpan.FromSeconds(10000);

            hubConnection.On<Message>("Send", message =>
            {

                MessageBus.Send(message);
                return Task.CompletedTask;

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
                DisconectSeverReloader.Reload();
                return Task.CompletedTask;
            };

        

            //возникает когда получается обратно подключится
            hubConnection.Reconnected += id =>
            {
                MessageBox.Show("Reconected Sucsesfull", "Сообщение");
                return Task.CompletedTask;
            };

            //возникает в момент переподключения
            hubConnection.Reconnecting += error =>
            {
             //   MessageBox.Show("Reconnecting", "Сообщение");
                return Task.CompletedTask;
            };

            return hubConnection.StartAsync();

        }



        

        
    }
}
