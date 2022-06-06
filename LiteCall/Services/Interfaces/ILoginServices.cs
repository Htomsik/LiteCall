using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces
{
    public interface ILoginServices
    {
        Task<bool> Login(bool isSeverAuthorise, Account _NewAccount,string _ApiServeIp);
    }
}
