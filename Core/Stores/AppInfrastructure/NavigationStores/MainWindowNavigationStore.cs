using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

public sealed class MainWindowNavigationStore
{
    private BaseVmd? _mainWindowCurrentViewModel;

    public BaseVmd? MainWindowCurrentViewModel
    {
        get => _mainWindowCurrentViewModel;
        set
        {
            _mainWindowCurrentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}