using System.Text.Json.Serialization;

namespace LiteCall.Model.ServerModels;

internal sealed class Server
{
    [JsonPropertyName("ip")] public string? Ip { get; set; }

    [JsonPropertyName("ApiIp")] public string? ApiIp { get; set; }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("country")] public string? Country { get; set; }

    [JsonPropertyName("city")] public string? City { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }
}