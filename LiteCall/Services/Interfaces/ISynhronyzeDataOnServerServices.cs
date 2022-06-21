using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal interface ISynhronyzeDataOnServerServices
    {
        public Task<bool> SaveOnServer();

        public Task<bool> GetFromServer();
    }
}
