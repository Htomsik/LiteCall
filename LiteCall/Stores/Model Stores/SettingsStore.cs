using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class SettingsStore:BaseVMD
    {

        public SettingsStore()
        {
            CurrentSettings.CurrentsettingsChanged += OnCurentSettingChanged;
        }

        public event Action CurentSettingChanged;

        private void OnCurentSettingChanged()
        {
            CurentSettingChanged?.Invoke();
        }

        private Settings _CurrentSettings = new Settings{ };

        public virtual Settings CurrentSettings
        {
            get => _CurrentSettings;
            set
            {
                Set(ref _CurrentSettings, value);
                OnCurentSettingChanged();
            }
        }




       
    }
}
