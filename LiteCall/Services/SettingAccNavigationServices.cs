using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services
{
    internal class SettingAccNavigationServices<TViewModel> : INavigationService where TViewModel : BaseVMD
    {
        private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

        private readonly Func<TViewModel> _CreateViewModel;

        public SettingAccNavigationServices(SettingsAccNavigationStore settingsAccNavigationStore, Func<TViewModel> createViewModel)
        {
            _settingsAccNavigationStore = settingsAccNavigationStore;

            _CreateViewModel = createViewModel;
        }

        public void Navigate()
        {
            _settingsAccNavigationStore.SettingsAccCurrentViewModel = _CreateViewModel();
        }
    }
}
