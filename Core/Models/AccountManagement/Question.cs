using System.Text.Json.Serialization;

namespace Core.Models.AccountManagement;

public class Question
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("questions")] public string? Text { get; set; }
}