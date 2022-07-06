using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class SettingAccNavigationSc<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

    public SettingAccNavigationSc(SettingsAccNavigationStore settingsAccNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _settingsAccNavigationStore = settingsAccNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _settingsAccNavigationStore.SettingsAccCurrentViewModel = _createViewModel();
    }
}