using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class ModalNavigationStore
    {
        private BaseVMD _ModalMainWindowCurrentViewModel;
        public BaseVMD ModalMainWindowCurrentViewModel
        {
            get => _ModalMainWindowCurrentViewModel;
            set
            {


                _ModalMainWindowCurrentViewModel?.Dispose();
                _ModalMainWindowCurrentViewModel = value;

                OnCurrentViewModelChanged();
            }
        }


        public void Close()
        {
            ModalMainWindowCurrentViewModel = null;
        }


        public bool IsOpen => ModalMainWindowCurrentViewModel != null;

        public event Action CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
