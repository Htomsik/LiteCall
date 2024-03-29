﻿using Core.Models.AppInfrastructure;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.Stores.AppInfrastructure;

public class AppExecutionStateStore:BaseVmd
{
    private AppExecutionState? _currentStatusMessage;

    public AppExecutionState? CurrentStatusMessage
    {
        get => _currentStatusMessage;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentStatusMessage, value);
            OnCurrentStatusMessageChanged();
        }
    }

    public bool IsOpen => !string.IsNullOrEmpty(CurrentStatusMessage?.Message);

    public event Action? CurrentStatusMessageChanged;

    private void OnCurrentStatusMessageChanged()
    {
        CurrentStatusMessageChanged?.Invoke();
    }
}