using System;

namespace LiteCall.Infrastructure.Bus;

internal static class ReloadServerRooms
{
    public static event Action? Reloader;

    public static void Reload()
    {
        Reloader?.Invoke();
    }
}