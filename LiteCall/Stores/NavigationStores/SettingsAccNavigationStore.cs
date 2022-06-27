using System;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class SettingsAccNavigationStore
{
    private BaseVmd? _settingsAccCurrentViewModel;

    public BaseVmd? SettingsAccCurrentViewModel
    {
        get => _settingsAccCurrentViewModel;
        set
        {
            _settingsAccCurrentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;


    public void Close()
    {
        SettingsAccCurrentViewModel = null;
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}