using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace LiteCall.Services
{
    public  class EncryptServices:IEncryptServices
    {
        public string Sha1Encrypt(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;

            using var sha1 = new SHA1Managed();

            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

     
    }
}
