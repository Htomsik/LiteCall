using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

public sealed class ModalNavigationStore
{
    private BaseVmd? _modalMainWindowCurrentViewModel;

    public BaseVmd? ModalMainWindowCurrentViewModel
    {
        get => _modalMainWindowCurrentViewModel;
        set
        {
            _modalMainWindowCurrentViewModel?.Dispose();
            _modalMainWindowCurrentViewModel = value;

            OnCurrentViewModelChanged();
        }
    }


    public bool IsOpen => ModalMainWindowCurrentViewModel != null;


    public void Close()
    {
        ModalMainWindowCurrentViewModel = null;
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}