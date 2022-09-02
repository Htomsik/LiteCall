using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;

namespace Core.Services.AppInfrastructure.NavigationServices;

public sealed class SettingAccNavigationSc<TViewModel> : INavigationSc where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly SettingsAccountVmdNavigationStore _settingsAccountVmdNavigationStore;

    public SettingAccNavigationSc(SettingsAccountVmdNavigationStore settingsAccountVmdNavigationStore,
        Func<TViewModel> createViewModel)
    {
        _settingsAccountVmdNavigationStore = settingsAccountVmdNavigationStore;

        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _settingsAccountVmdNavigationStore.CurrentValue = _createViewModel();
    }
}