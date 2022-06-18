using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces
{
    internal interface IhttpDataServices
    {
        public Task<string> GetAuthorizeToken(Account newAcc, string apiServerIp = null);

        public Task<string> Registration(Account newAcc, string capthca, string apiServerIp = null);

        public Task<string> MainServerGetApiIP(string serverName);

        public Task<Server> ApiServerGetInfo(string apiServerIp);

        public Task<ImagePacket> GetCaptcha(string serverIp = null);

        public Task<bool> CheckServerStatus(string serverIp);

        public Task<string> GetRoleFromJwtToken(string token);
    }
}
