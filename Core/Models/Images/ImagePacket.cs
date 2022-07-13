using System.Text.Json.Serialization;

namespace Core.Models.Images;

public class ImagePacket
{
    
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

    [JsonPropertyName("len")]
    public int Lenght { get; set; }

    [JsonPropertyName("image")]
    public string Bytes { get; set; } = string.Empty;

    
}