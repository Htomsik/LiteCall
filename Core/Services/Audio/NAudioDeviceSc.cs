using Core.Models.Audio;
using Core.Services.Interfaces.Audio;
using NAudio.CoreAudioApi;
using NAudio.Wave;


namespace Core.Services.Audio;

public class NAudioDeviceSc : IAudioDeviceSc
{
    public IEnumerable<AudioDevice> GetInputDevices()
    {
        var devices = new List<AudioDevice>();
        
        var enumerator = new MMDeviceEnumerator();
        var mmuDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
        
        int deviceCount = WaveIn.DeviceCount;
        for (int i = 0; i < deviceCount; i++)
        {
            var waveDevice = WaveIn.GetCapabilities(i);
            var mmuDevice = mmuDevices.FirstOrDefault(x => x.FriendlyName.Contains(waveDevice.ProductName));
            
            devices.Add(new AudioDevice
            {
                Name = mmuDevice != null ?  mmuDevice.FriendlyName : waveDevice.ProductName,
                Id = i,
            });
        }

        return devices;
    }

    public IEnumerable<AudioDevice> GetOutputDevices()
    {
        var devices = new List<AudioDevice>();
        
        var enumerator = new MMDeviceEnumerator();
        var mmuDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        
        int deviceCount = WaveOut.DeviceCount;
        for (int i = 0; i < deviceCount; i++)
        {
            var waveDevice = WaveOut.GetCapabilities(i);
            var mmuDevice = mmuDevices.FirstOrDefault(x => x.FriendlyName.Contains(waveDevice.ProductName));
            
            devices.Add(new AudioDevice
            {
                Name = mmuDevice != null ?  mmuDevice.FriendlyName : waveDevice.ProductName,
                Id = i,
            });
        }

        return devices;
    }
}