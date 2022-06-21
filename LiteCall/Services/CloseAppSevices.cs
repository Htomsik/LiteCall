using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal class CloseAppSevices:ICloseAppSevices
    {
        private readonly ISynhronyzeDataOnServerServices _synhronyzeDataOnServerServices;


        public CloseAppSevices(ISynhronyzeDataOnServerServices synhronyzeDataOnServerServices)
        {
            _synhronyzeDataOnServerServices = synhronyzeDataOnServerServices;
        }
        public async Task Close()
        {

          await  _synhronyzeDataOnServerServices?.SaveOnServer()!;

            Application.Current.Shutdown();
        }
    }
}
