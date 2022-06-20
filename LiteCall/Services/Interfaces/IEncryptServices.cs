using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Services.Interfaces
{
    public interface IEncryptServices
    {
        public  Task<string> Sha1Encrypt(string content);

        public  Task<string> Base64Encypt(string content);

        public  Task<string> Base64Decrypt(string content);
    }
}
