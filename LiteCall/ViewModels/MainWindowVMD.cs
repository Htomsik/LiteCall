using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels
{   
    internal class MainWindowVMD:BaseVMD
    {
        public MainWindowVMD(NavigationStore navigationStore)
        {
            _NavigationStore = navigationStore;
        }



        private readonly NavigationStore _NavigationStore;

        public BaseVMD CurrentViewModel => _NavigationStore.MainWindowCurrentViewModel;
    }
}
