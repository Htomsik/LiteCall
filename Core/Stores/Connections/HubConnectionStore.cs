using Microsoft.AspNetCore.SignalR.Client;

namespace Core.Stores.Connections;

public class HubConnectionStore
{
    public HubConnection? CurrentHubConnection { get; set; }
    
    public async Task StopConnection()
    {
        await CurrentHubConnection!.StopAsync();

        await CurrentHubConnection.DisposeAsync();
    }
}