using System;
using LiteCall.ViewModels.Base;
using Newtonsoft.Json;

namespace LiteCall.Model;

internal class Settings : BaseVmd
{
    [JsonIgnore] private int _captureDeviceId;

    [JsonIgnore] private int _outputDeviceId;

    public int OutputDeviceId
    {
        get => _outputDeviceId;
        set
        {
            Set(ref _outputDeviceId, value);
            OnCurrentSettingsChanged();
        }
    }

    public int CaptureDeviceId
    {
        get => _captureDeviceId;
        set
        {
            Set(ref _captureDeviceId, value);
            OnCurrentSettingsChanged();
        }
    }


    private void OnCurrentSettingsChanged()
    {
        CurrentSettingsChanged?.Invoke();
    }

    public event Action? CurrentSettingsChanged;
}