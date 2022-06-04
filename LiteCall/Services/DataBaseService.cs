using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using LiteCall.Model;
using LiteCall.Stores;
using Microsoft.AspNetCore.WebUtilities;
using MessageBox = System.Windows.MessageBox;

namespace LiteCall.Services
{
    internal static class DataBaseService
    {
        private static Guid ProgramCaptchaID = Guid.NewGuid();

        const string apikey = "ACbaAS324hnaASD324bzZwq41";

        internal static async Task<string> GetAuthorizeToken(Account newAcc)
        {
            using var httpClient = new HttpClient();

            var authModel = new { Login = newAcc.Login, Password = newAcc.Password.GetHashSha1(),Guid = ProgramCaptchaID };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            httpClient.DefaultRequestHeaders.Add("ApiKey", apikey);

            var response = new HttpResponseMessage();

            try
            {
                response = await httpClient.PostAsync("http://localhost:5000/api/Auth/Authorization", content).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return "invalid";
            }

            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {

               MessageBox.Show(response.Content.ReadAsStringAsync().Result, "Сообщение");

                return "invalid";
            }

        }

        internal static async Task<string> Registration(Account newAcc, string capthca, string ApiServerIp = "localhost:5000")
        {
            using var httpClient = new HttpClient();

            var authModel = new { Login = newAcc.Login, Password = newAcc.Password.GetHashSha1(),Guid = ProgramCaptchaID,Captcha = capthca };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            httpClient.DefaultRequestHeaders.Add("ApiKey", apikey);

            var response = await httpClient.PostAsync("http://localhost:5000/api/auth/Registration", content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Content.ToString();
            }
            else
            {
                MessageBox.Show(response.Content.ReadAsStringAsync().Result, "Сообщение");

                return response.ReasonPhrase;
            }
        }




        internal static async Task<Server> ServerGetInfo(string ServerName,string ApiServerIp = "localhost:5000")
        {
            
                using var httpClient = new HttpClient();

                var json = JsonSerializer.Serialize(ServerName);

                var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);


                httpClient.DefaultRequestHeaders.Add("ApiKey", apikey);

            var response = new HttpResponseMessage();
                try
                {
                    response = await httpClient.PostAsync($"http://{ApiServerIp}/api/ServerList/ServerGetInfo", content).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Сообщение");
                    return null;
                }
               

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


        internal static async Task<Server> ServerGetInfo(Server server)
        {
            using var httpClient = new HttpClient();

            var json = JsonSerializer.Serialize(server.Ip);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            httpClient.DefaultRequestHeaders.Add("ApiKey", apikey);

            var response = await httpClient.PostAsync("http://localhost:5000/api/ServerList/ServerGetInfo", content).ConfigureAwait(false);

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




        public static async Task<ImagePacket> GetCaptcha()
        {
            var httpClient = new HttpClient();

            //var authModel = new { Login = login, Password = GetHashSha1(password) };

            var json = JsonSerializer.Serialize(ProgramCaptchaID.ToString());

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            httpClient.DefaultRequestHeaders.Add("ApiKey", apikey);

            var response = await httpClient.PostAsync("http://localhost:5000/api/auth/CaptchaGenerator", content).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<ImagePacket>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                return null;
            }
        }

        private static string GetHashSha1(this string content)
        {
            if (string.IsNullOrEmpty(content)) return "X";
            using var sha1 = new SHA1Managed();

            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public static BitmapSource GetImageStream(System.Drawing.Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpPt,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            //DeleteObject(bmpPt);

            return bitmapSource;
        }



        public static bool IsAuthorize(string token)
        {

            if (string.IsNullOrEmpty(token)) return false;
            try
            {
                dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.Split('.')[1])));
                return (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] != "Anonymous" ? true : false;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return false;
            }


        }



    }
}
