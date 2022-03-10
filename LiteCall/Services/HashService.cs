using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiteCall.Services
{
    internal static class HashService
    {
        internal static string GetHashSha1(this string content)
        {
            byte[] hash;

            using var sha1 = new SHA1Managed();

            hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }


    }
}



