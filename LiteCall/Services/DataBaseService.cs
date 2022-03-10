using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal static class DataBaseService
    {
        internal static async Task<string> GetAuthorizeToken(Account newAcc)
        {
            using var httpClient = new HttpClient();


            var authModel = new { Login = newAcc.Login, Password = newAcc.Password.GetHashSha1() };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync("http://localhost:57785/api/auth/token", content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return string.Empty;

            }
        }

        internal static async Task<string> Registration(Account newAcc)
        {
            using var httpClient = new HttpClient();

            var authModel = new { Login = newAcc.Login, Password = newAcc.Password.GetHashSha1() };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync("http://localhost:57785/api/auth/Registration", content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return string.Empty;
            }
        }




        internal static async Task<Server> ServerGetInfo(string ServerName)
        {
            using var httpClient = new HttpClient();

            var json = JsonSerializer.Serialize(ServerName);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync("http://localhost:57785/api/ServerList/ServerGetInfo", content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                var a = response.Content.ReadAsStringAsync().Result;

                var str = JsonSerializer.Deserialize<Server>(a);

                return str;
            }
            else
            {
                return null;
            }
        }

        private static string GetHashSha1(this string content)
            {

                using var sha1 = new SHA1Managed();

                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

                return string.Concat(hash.Select(b => b.ToString("x2")));
            }


        
    }
}
