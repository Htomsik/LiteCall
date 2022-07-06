using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure.NavigationStores;

public sealed class SettingsAccNavigationStore
{
    private BaseVmd? _settingsAccCurrentViewModel;

    public BaseVmd? SettingsAccCurrentViewModel
    {
        get => _settingsAccCurrentViewModel;
        set
        {
            _settingsAccCurrentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;


    public void Close()
    {
        SettingsAccCurrentViewModel = null;
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}