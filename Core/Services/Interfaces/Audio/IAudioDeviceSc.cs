using Core.Models.Audio;

namespace Core.Services.Interfaces.Audio;

/// <summary>
///     Audio device manager
/// </summary>
public interface IAudioDeviceSc
{
    IEnumerable<AudioDevice> GetInputDevices();

    IEnumerable<AudioDevice> GetOutputDevices();
}