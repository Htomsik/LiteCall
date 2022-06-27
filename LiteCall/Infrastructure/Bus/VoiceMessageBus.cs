using System;
using LiteCall.Model.ServerModels.Messages;

namespace LiteCall.Infrastructure.Bus;

internal static class VoiceMessageBus
{
    public static event Action<VoiceMessage>? Bus;

    public static void Send(VoiceMessage data)
    {
        Bus?.Invoke(data);
    }
}