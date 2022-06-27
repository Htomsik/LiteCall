using System;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class SettingsStore : BaseVmd
{
    private Settings? _currentSettings = new();

    public SettingsStore()
    {
        CurrentSettings!.CurrentSettingsChanged += OnCurrentSettingsChanged;
    }

    public Settings? CurrentSettings
    {
        get => _currentSettings;
        set
        {
            Set(ref _currentSettings, value);
            OnCurrentSettingsChanged();
        }
    }

    public event Action? CurrentSettingsChanged;

    private void OnCurrentSettingsChanged()
    {
        CurrentSettingsChanged?.Invoke();
    }
}