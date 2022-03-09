
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services;

namespace SignalRServ
{
    public class ServerService
    {
        public static HubConnection hubConnection;
        public static Task ConnectionHub(string url/*,Action<Message> a*/)
        {
            hubConnection = new HubConnectionBuilder()
                 .WithUrl(url)
                 .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, 
                     TimeSpan.Zero,TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(15),TimeSpan.FromSeconds(50)})
                 .Build();

            hubConnection.ServerTimeout = TimeSpan.FromSeconds(1800);
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


            //если соединение закрыто
            hubConnection.Closed += error =>
            {
                Console.WriteLine($"Connection closed {error.Message}");
                return Task.CompletedTask;
            };

            //возникает когда получается обратно подключится
            hubConnection.Reconnected += id =>
            {
                Console.WriteLine($"Connection reconected with id {id}");
                return Task.CompletedTask;
            };

            //возникает в момент переподключения
            hubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"Connection reconecting {error.Message}");
                return Task.CompletedTask;
            };
            return hubConnection.StartAsync();

        }
    }
}
