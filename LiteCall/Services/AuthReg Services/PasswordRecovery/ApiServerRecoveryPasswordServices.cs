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
    internal class ApiServerRecoveryPasswordServices:IRecoveryPasswordServices
    {
        private readonly IhttpDataServices _httpDataServices;

        private readonly CurrentServerStore _currentServerStore;


        public ApiServerRecoveryPasswordServices(IhttpDataServices httpDataServices,CurrentServerStore currentServerStore)
        {
            _httpDataServices = httpDataServices;
            _currentServerStore = currentServerStore;
        }

        public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
        {
            return await _httpDataServices.PasswordRecovery(recoveryModel,_currentServerStore.CurrentServer.ApiIp);
        }

      
    }
}
