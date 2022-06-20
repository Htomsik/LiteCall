using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Services.Interfaces
{
    public interface IEncryptServices
    {
        public  string Sha1Encrypt(string content);
    }
}
