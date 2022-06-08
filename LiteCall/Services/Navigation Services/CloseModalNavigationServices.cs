using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.NavigationServices
{
    
        internal class CloseModalNavigationServices:INavigationService
        {
            private readonly ModalNavigationStore _modalNavigationStore;


            public CloseModalNavigationServices(ModalNavigationStore modalNavigationStore)
            {
                _modalNavigationStore = modalNavigationStore;

            }

            public void Navigate()
            {
                _modalNavigationStore.Close();
            }
        }
    
}
