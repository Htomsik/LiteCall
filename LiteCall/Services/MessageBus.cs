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
    internal static class ReloadServer
    {
        public static event Action Reloader;

        public static void Reload()
        {
            Reloader?.Invoke();
        }

    }



}
