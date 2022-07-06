namespace Core.Infrastructure.Notifiers;

public static class KickFromRoomNotifier
{
    public static event Action? Notificator;

    public static void Notify()
    {
        Notificator?.Invoke();
    }
}