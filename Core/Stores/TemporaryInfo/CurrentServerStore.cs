﻿using System.Collections.ObjectModel;
using Core.Models.Servers;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.Stores.TemporaryInfo;

public sealed class CurrentServerStore : BaseVmd
{
    private Server? _currentServer;
    
    private ObservableCollection<ServerRooms>? _currentSeverRooms = new();

    public Server? CurrentServer
    {
        get => _currentServer;
        set
        {
            var oldvalue = _currentServer;
            
            this.RaiseAndSetIfChanged(ref _currentServer, value);
            
            if(oldvalue!= value && value == null) 
                OnCurrentServerDeleted();
            
            OnCurrentServerChanged();
        }
    }

    public ObservableCollection<ServerRooms>? CurrentServerRooms
    {
        get => _currentSeverRooms;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentSeverRooms, value);
            CurrentServerRoomsChanged?.Invoke();
        }
    }


    public event Action? CurrentServerRoomsChanged;
    
    public event Action? CurrentServerDeleted;
    
    public event Action? CurrentServerChanged;

    private void OnCurrentServerChanged()
    {
        CurrentServerChanged?.Invoke();
    }

    private void OnCurrentServerDeleted()
    {
        CurrentServerDeleted?.Invoke();
    }


    public Task Delete()
    {
        CurrentServer = null;
        return Task.CompletedTask;
    }
}