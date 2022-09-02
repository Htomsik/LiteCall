using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.Servers;

public  sealed class ServerRooms : ReactiveObject
{
    [Reactive]
    public string? RoomName { get; set; }

    [Reactive]
    public bool WithPassword { get; set; }

    [Reactive]
    public ICollection<ServerUser>? Users { get; set; }
    
}