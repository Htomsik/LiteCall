namespace Core.Models.ServerModels.Messages;

public class TextMessage
{
    public string? Text { get; set; }
    
    public string? Sender { get; set; }
    
    public DateTime DateSend { get; set; }
}