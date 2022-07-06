using Core.Models.Servers.Messages;

namespace Core.Infrastructure.Buses;

public static class AudioMessageBus
{
    public static event Action<AudioMessage>? Bus;

    public static void Send(AudioMessage data)
    {
        Bus?.Invoke(data);
    }
}