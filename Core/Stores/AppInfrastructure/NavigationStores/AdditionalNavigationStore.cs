using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

public sealed class AdditionalNavigationStore
{
    private BaseVmd? _additionalMainWindowCurrentViewModel;

    public BaseVmd? AdditionalMainWindowCurrentViewModel
    {
        get => _additionalMainWindowCurrentViewModel;
        set
        {
            _additionalMainWindowCurrentViewModel?.Dispose();
            _additionalMainWindowCurrentViewModel = value;

            OnCurrentViewModelChanged();
        }
    }


    public bool IsOpen => AdditionalMainWindowCurrentViewModel != null;


    public void Close()
    {
        AdditionalMainWindowCurrentViewModel = null;
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}