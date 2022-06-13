using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces
{
    public interface IAuthorisationServices
    {
        Task<int> Login(bool IsAuthTokenAuthorise, Account _NewAccount, string ApiServerIp = null);


    }
}
