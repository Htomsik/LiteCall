using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services
{
    internal class MainServerRecoveryPasswordServices:IRecoveryPasswordServices
    {
        private readonly IhttpDataServices _httpDataServices;

        public MainServerRecoveryPasswordServices(IhttpDataServices httpDataServices)
        {
            _httpDataServices = httpDataServices;
        }

        public async Task<bool> RecoveryPassword(RecoveryModel recoveryModel)
        {
            return await _httpDataServices.PasswordRecovery(recoveryModel);
        }

       
    }
}
