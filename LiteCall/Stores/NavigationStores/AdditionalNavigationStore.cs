using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class AdditionalNavigationStore
    {
        private BaseVMD _AdditionalMainWindowCurrentViewModel;
        public BaseVMD AdditionalMainWindowCurrentViewModel
        {
            get => _AdditionalMainWindowCurrentViewModel;
            set
            {


                _AdditionalMainWindowCurrentViewModel?.Dispose();
                _AdditionalMainWindowCurrentViewModel = value;

                OnCurrentViewModelChanged();
            }
        }


        public void Close()
        {
            AdditionalMainWindowCurrentViewModel = null;
        }


        public bool IsOpen => AdditionalMainWindowCurrentViewModel != null;

        public event Action CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
