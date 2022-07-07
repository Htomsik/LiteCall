using Core.VMD.Base;
using ReactiveUI;

namespace Core.Models.Servers;

public  sealed class ServerRooms : BaseVmd
{
    private ICollection<ServerUser>? _users;

    public string? RoomName { get; set; }

    public bool Guard { get; set; }

    public ICollection<ServerUser>? Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }
}