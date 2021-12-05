using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class NavigationStore
    {
        private BaseVMD _MainWindowCurrentViewModel;
        public BaseVMD MainWindowCurrentViewModel
        {
            get => _MainWindowCurrentViewModel;
            set
            {
               
                _MainWindowCurrentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public event Action CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }


        
    }
}
