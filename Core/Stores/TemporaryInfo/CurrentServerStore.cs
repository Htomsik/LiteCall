using System.Collections.ObjectModel;
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
            this.RaiseAndSetIfChanged(ref _currentServer, value);
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