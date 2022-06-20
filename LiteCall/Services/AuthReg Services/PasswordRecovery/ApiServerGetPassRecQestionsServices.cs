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
    internal class ApiServerGetPassRecQestionsServices : IGetPassRecoveryQuestionsServices
    {
        private readonly IhttpDataServices _httpDataServices;

        private readonly CurrentServerStore _currentServerStore;


        public ApiServerGetPassRecQestionsServices(IhttpDataServices httpDataServices, CurrentServerStore currentServerStore)
        {
            _httpDataServices = httpDataServices;

            _currentServerStore = currentServerStore;
        }

        public async Task<List<Question>> GetQestions()
        {
            return await _httpDataServices.GetPasswordRecoveryQestions(_currentServerStore.CurrentServer.ApiIp);
        }
    }
}
