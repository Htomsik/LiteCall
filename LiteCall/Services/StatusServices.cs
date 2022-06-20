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

        private static bool _isDelete = false;
        public StatusServices(StatusMessageStore statusMessageStore)
        {
            _statusMessageStore = statusMessageStore;
        }

        public async  void ChangeStatus(StatusMessage newStatusMessage)
        {

            if (!_isDelete)
            {
                _statusMessageStore.CurentStatusMessage = newStatusMessage;
            }
            else
            {
                return;
            }
            if (newStatusMessage.isError) 
            {
                _isDelete = true;

                await  TimerDelete(4000);

              
            }
           
        }

        private async Task TimerDelete(int Delay)
        {

            await Task.Delay(Delay);

            _isDelete = false;

            DeleteStatus();

        }

        public async void DeleteStatus()
        {
                
            if(_isDelete) return;

            _statusMessageStore.CurentStatusMessage = null;

            
        }
    }
}
