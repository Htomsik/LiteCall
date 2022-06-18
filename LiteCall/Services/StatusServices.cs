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

        public  void ChangeStatus(StatusMessage newStatusMessage)
        {
            _statusMessageStore.CurentStatusMessage = newStatusMessage;

            if (newStatusMessage.isError)
            {
                TimerDelete();
            }
        }

        private async void TimerDelete()
        {
             await Task.Delay(7000);

             DeleteStatus();
        }

        public void DeleteStatus()
        {
            _statusMessageStore.CurentStatusMessage = null;
        }
    }
}
