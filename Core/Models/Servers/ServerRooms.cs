using Core.VMD.Base;

namespace Core.Models.Servers;

public  sealed class ServerRooms : BaseVmd
{
    private ICollection<ServerUser>? _users;

    public string? RoomName { get; set; }

    public bool Guard { get; set; }

    public ICollection<ServerUser>? Users
    {
        get => _users;
        set => Set(ref _users, value);
    }
}