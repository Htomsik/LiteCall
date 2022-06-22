using System;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices;

internal class SettingAccNavigationServices<TViewModel> : INavigationService where TViewModel : BaseVmd
{
    private readonly Func<TViewModel> _createViewModel;
    private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

    public SettingAccNavigationServices(SettingsAccNavigationStore settingsAccNavigationStore,
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