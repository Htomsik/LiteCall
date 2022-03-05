
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
            
            hubConnection.On<Message>("Send", message => 
            {                                         
                
                
               
            });
            hubConnection.On("UpdateRooms", () =>
            {

                ReloadServerRooms.Reload();
                return Task.CompletedTask;

            });

            hubConnection.Closed += error =>
            {
                Console.WriteLine($"Connection closed {error.Message}");
                return Task.CompletedTask;
            };

            hubConnection.Reconnected += id =>
            {
                Console.WriteLine($"Connection reconected with id {id}");
                return Task.CompletedTask;
            };
            hubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"Connection reconecting {error.Message}");
                return Task.CompletedTask;
            };
            return hubConnection.StartAsync();

        }
    }
}
