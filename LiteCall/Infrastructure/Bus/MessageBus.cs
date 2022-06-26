using System;
using LiteCall.Model;

namespace LiteCall.Infrastructure.Bus;

internal static class MessageBus
{
    public static event Action<Message>? Bus;

    public static void Send(Message data)
    {
        Bus?.Invoke(data);
    }
}
