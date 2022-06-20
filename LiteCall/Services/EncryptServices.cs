﻿using System;
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

        readonly static byte[] Entropy = { 1,2,3,4,5,6,7,8,9,10 };

        public async Task<string> Sha1Encrypt(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;

            using var sha1 = new SHA1Managed();

            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public async Task<string> Base64Encypt(string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            byte[] originalText = Encoding.Unicode.GetBytes(content);

         
            byte[] encryptedText = ProtectedData.Protect(originalText, Entropy, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedText);
        }

        public async Task<string> Base64Decrypt(string content)
        {
         
            if(string.IsNullOrEmpty(content)) return content;

            byte[] encryptedText = Convert.FromBase64String(content);

           
            byte[] originalText = ProtectedData.Unprotect(encryptedText, Entropy, DataProtectionScope.CurrentUser);

           
            return Encoding.Unicode.GetString(originalText);
        }
    }
}
