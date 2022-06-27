using System.Text.Json.Serialization;

namespace LiteCall.Model;

public class Question
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("questions")] public string? Text { get; set; }
}