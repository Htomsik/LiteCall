using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LiteCall.Stores;

namespace LiteCall.Services
{
    internal static class TokenService
    {
        internal static async Task<string> GetToken(AccountStore accStore)
        {
            using var httpClient = new HttpClient();


            var authModel = new { Login = accStore.CurrentAccount.Login, Password = accStore.CurrentAccount.Password.GetHashSha1() };

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
    }
}
