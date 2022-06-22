using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiteCall.ViewModels.Base;

namespace LiteCall.Model;

internal class Server
{
    [JsonPropertyName("ip")] public string? Ip { get; set; }

    [JsonPropertyName("ApiIp")] public string? ApiIp { get; set; }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("country")] public string? Country { get; set; }

    [JsonPropertyName("city")] public string? City { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }
}

internal class ServerRooms : BaseVmd
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