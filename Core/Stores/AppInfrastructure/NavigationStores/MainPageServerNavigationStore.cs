using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

public sealed class MainPageServerNavigationStore
{
    private BaseVmd? _mainPageServerCurrentViewModel;

    public BaseVmd? MainPageServerCurrentViewModel
    {
        get => _mainPageServerCurrentViewModel;
        set
        {
            _mainPageServerCurrentViewModel?.Dispose();
            _mainPageServerCurrentViewModel = value;

            OnCurrentViewModelChanged();
        }
    }
    
    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}