using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores.ModelStores;

namespace LiteCall.Services
{
    internal class StatusServices:IStatusServices
    {
        private StatusMessageStore _statusMessageStore;

        public StatusServices(StatusMessageStore statusMessageStore)
        {
            _statusMessageStore = statusMessageStore;
        }

        public async  void ChangeStatus(StatusMessage newStatusMessage)
        {
            _statusMessageStore.CurentStatusMessage = newStatusMessage;

            if (newStatusMessage.isError)
            {
              await  TimerDelete(7000);
            }
           
        }

        private async Task TimerDelete(int Delay)
        {
             await Task.Delay(Delay);

             DeleteStatus();
        }

        public void DeleteStatus()
        {
            _statusMessageStore.CurentStatusMessage = null;
        }
    }
}
