using Core.Models.ServerModels.Messages;

namespace Core.Infrastructure.Buses;

public static class TextMessageBus
{
    public static event Action<TextMessage>? Bus;

    public static void Send(TextMessage data)
    {
        Bus?.Invoke(data);
    }
}