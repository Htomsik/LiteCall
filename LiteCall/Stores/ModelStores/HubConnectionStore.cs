using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace LiteCall.Stores;

public class HubConnectionStore
{
    public HubConnection? CurrentHubConnection { get; set; }


    public async Task StopConnection()
    {
        await CurrentHubConnection!.StopAsync();

        await CurrentHubConnection.DisposeAsync();
    }
}