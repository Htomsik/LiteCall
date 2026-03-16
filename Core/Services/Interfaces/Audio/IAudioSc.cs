using Core.Models.Servers.Messages;

namespace Core.Services.Interfaces.Audio;

/// <summary>
///     Voice chat audio service
/// </summary>
public interface IAudioSc
{
    void StartRecording();
    
    void StopRecording();

    void ClearAudio();
    
    void PlayMessage(AudioMessage audioMessage);
    
    event Action<byte[]>? InputDataGenerated;
}