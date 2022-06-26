using System;

namespace LiteCall.Model.ServerModels.Messages;

public class Message
{
    public string? Text { get; set; }
    public string? Sender { get; set; }
    public DateTime DateSend { get; set; }
}