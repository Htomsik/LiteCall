using System.Collections.Generic;
using LiteCall.Model.Users;
using LiteCall.ViewModels.Base;

namespace LiteCall.Model.ServerModels;

internal sealed class ServerRooms : BaseVmd
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