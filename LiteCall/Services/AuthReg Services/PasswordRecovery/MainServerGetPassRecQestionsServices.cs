using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services
{
    internal class MainServerGetPassRecQestionsServices : IGetPassRecoveryQuestionsServices
    {
        private readonly IhttpDataServices _httpDataServices;


        public MainServerGetPassRecQestionsServices(IhttpDataServices httpDataServices)
        {
            _httpDataServices = httpDataServices;
        }

        public async Task<List<Question>> GetQestions()
        {
            return await _httpDataServices.GetPasswordRecoveryQestions();
        }
    }
}
