using System.Collections.ObjectModel;
using Core.Models.Audio;
using Core.Services.Interfaces.Audio;
using Core.Stores.AppInfrastructure;
using Core.VMD.AdditionalVmds.SettingsVmds.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.AdditionalVmds.SettingsVmds;

/// <summary>
///      Manage Audio vmd in Settings vmd
/// </summary>
public class AudioSettingsVmd : BaseSettingsVmd
{
    [Reactive]
    public ObservableCollection<AudioDevice> InputDevices { get; set; }
    
    [Reactive]
    public ObservableCollection<AudioDevice> OutputDevices { get; set; }
    
    public AudioDevice? CaptureDevice
    {
        get => InputDevices.FirstOrDefault(x => x.Id == _lazyAppSettingsStore.Value.CurrentValue.CaptureDeviceId);
        set => _lazyAppSettingsStore.Value.CurrentValue.CaptureDeviceId = value?.Id ?? 0;
    }
    
    public AudioDevice? OutputDevice
    {
        get => OutputDevices.FirstOrDefault(x => x.Id == _lazyAppSettingsStore.Value.CurrentValue.OutputDeviceId);
        set => _lazyAppSettingsStore.Value.CurrentValue.OutputDeviceId = value?.Id ?? 0;
    }

    private readonly IAudioDeviceSc _audioDeviceSc;
    
    public AudioSettingsVmd(AppSettingsStore appSettingsStore, 
        IAudioDeviceSc audioDeviceSc) : base(appSettingsStore)
    {
        _audioDeviceSc = audioDeviceSc;

        LoadDevices();
    }

    private void LoadDevices()
    {
        InputDevices = new ObservableCollection<AudioDevice>(_audioDeviceSc.GetInputDevices());
        OutputDevices = new ObservableCollection<AudioDevice>(_audioDeviceSc.GetOutputDevices());
    }
}