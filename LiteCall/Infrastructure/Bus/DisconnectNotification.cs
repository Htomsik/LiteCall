using System;

namespace LiteCall.Infrastructure.Bus;

internal static class DisconnectNotification
{
    public static event Action? Notificator;

    public static void Reload()
    {
        Notificator?.Invoke();
    }
}