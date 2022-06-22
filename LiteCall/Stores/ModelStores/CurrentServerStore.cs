using System;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class CurrentServerStore : BaseVmd
{
    private Server? _currentServer;

    public Server? CurrentServer
    {
        get => _currentServer;
        set
        {
            Set(ref _currentServer, value);
            OnCurrentServerChangeChanged();
        }
    }

    public event Action? CurrentServerChange;

    private void OnCurrentServerChangeChanged()
    {
        CurrentServerChange?.Invoke();
    }


    public void Delete()
    {
        CurrentServer = null;
    }
}