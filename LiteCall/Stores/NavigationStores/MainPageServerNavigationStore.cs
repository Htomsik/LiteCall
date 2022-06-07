using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores.NavigationStores
{
    internal class MainPageServerNavigationStore
    {
        private BaseVMD _MainPageServerCurrentViewModel;
        public BaseVMD MainPageServerCurrentViewModel
        {
            get => _MainPageServerCurrentViewModel;
            set
            {


                _MainPageServerCurrentViewModel?.Dispose();
                _MainPageServerCurrentViewModel = value;

                OnCurrentViewModelChanged();
            }
        }


        public void Close()
        {
            MainPageServerCurrentViewModel.Dispose();
        }


        public event Action CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
