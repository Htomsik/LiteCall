using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal class CloseAdditionalNavigationServices:INavigationService
    {
        private readonly AdditionalNavigationStore _navigationStore;

        public CloseAdditionalNavigationServices(AdditionalNavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public void Navigate()
        {
            _navigationStore.Close();
        }
    }
}
