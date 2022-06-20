using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services
{
    internal class MainServerGetPassRecQestions:IGetPasswordRecoveryQuestions
    {
        private readonly IhttpDataServices _httpDataServices;


        public MainServerGetPassRecQestions(IhttpDataServices httpDataServices)
        {
            _httpDataServices = httpDataServices;
        }

        public async Task<List<Question>> GetQestions()
        {
            return await _httpDataServices.GetPasswordRecoveryQestions();
        }
    }
}
