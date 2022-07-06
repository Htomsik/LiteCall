

namespace Core.Infrastructure.Notifiers;

public static class DisconnectFromServerNotificator
{
    public static event Action? Notificator;

    public static void Notify()
    {
        Notificator?.Invoke();
    }
}