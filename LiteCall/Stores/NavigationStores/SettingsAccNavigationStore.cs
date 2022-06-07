using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class SettingsAccNavigationStore
    {
        private BaseVMD _SettingsAccCurrentViewModel;
        public BaseVMD SettingsAccCurrentViewModel
        {
            get => _SettingsAccCurrentViewModel;
            set
            {

                _SettingsAccCurrentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public event Action CurrentViewModelChanged;


        public void Close()
        {
            SettingsAccCurrentViewModel = null;
        }
        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
