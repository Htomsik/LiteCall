using System;
using System.Collections.ObjectModel;
using LiteCall.Model.ServerModels;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class CurrentServerStore : BaseVmd
{
    private Server? _currentServer;


    private ObservableCollection<ServerRooms>? _currentSeverRooms = new();

    public Server? CurrentServer
    {
        get => _currentServer;
        set
        {
            Set(ref _currentServer, value);
            OnCurrentServerChanged();
        }
    }

    public ObservableCollection<ServerRooms>? CurrentServerRooms
    {
        get => _currentSeverRooms;
        set
        {
            Set(ref _currentSeverRooms, value);
            CurrentServerRoomsChanged?.Invoke();
        }
    }


    public event Action? CurrentServerRoomsChanged;

    public event Action? CurrentServerChanged;

    public event Action? CurrentServerDeleted;

    private void OnCurrentServerChanged()
    {
        CurrentServerChanged?.Invoke();
        if (CurrentServer is null)
            CurrentServerDeleted?.Invoke();
    }


    public void Delete()
    {
        CurrentServer = null;
    }
}