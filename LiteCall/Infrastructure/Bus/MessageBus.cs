using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services
{
    internal static class MessageBus
    {
        public static event Action<Message> Bus;

        public static void Send(Message data) => Bus?.Invoke(data);
    }



    internal static class VoiceMessageBus
    {
        public static event Action<VoiceMessage> Bus;

        public static void Send(VoiceMessage data) => Bus?.Invoke(data);
    }


    internal static class ReloadServerRooms
    {
        public static event Action Reloader;

        public static void Reload()
        {
            Reloader?.Invoke();
        }

    }

    internal static class DisconnectNotification
    {
        public static event Action Notificator;

        public static void Reload()
        {
            Notificator?.Invoke();
        }

    }


    internal static class DisconectSeverReloader
    {
        public static event Action Reloader;

        public static void Reload()
        {
            Reloader?.Invoke();
        }

    }



}
