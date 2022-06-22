using System;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class StatusMessageStore : BaseVmd
{
    private StatusMessage? _currentStatusMessage;

    public StatusMessage? CurrentStatusMessage
    {
        get => _currentStatusMessage;
        set
        {
            Set(ref _currentStatusMessage, value);
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