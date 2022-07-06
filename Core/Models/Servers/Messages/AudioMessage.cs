namespace Core.Models.Servers.Messages;

public class AudioMessage
{
    public string? UserName { get; set; }

    public byte[]? Audio { get; set; }
}