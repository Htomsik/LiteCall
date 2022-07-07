using Core.VMD.Base;
using Newtonsoft.Json;
using ReactiveUI;

namespace Core.Models.AppInfrastructure;

public class AppSettings:BaseVmd
{
    [JsonIgnore] private int _captureDeviceId;

    [JsonIgnore] private int _outputDeviceId;

    public int OutputDeviceId
    {
        get => _outputDeviceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _outputDeviceId, value);
            OnCurrentSettingsChanged();
        }
    }

    public int CaptureDeviceId
    {
        get => _captureDeviceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _captureDeviceId, value);
            OnCurrentSettingsChanged();
        }
    }


    private void OnCurrentSettingsChanged()
    {
        CurrentSettingsChanged?.Invoke();
    }

    public event Action? CurrentSettingsChanged;
}