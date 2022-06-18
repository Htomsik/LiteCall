using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using Microsoft.AspNetCore.WebUtilities;
using MessageBox = System.Windows.MessageBox;

namespace LiteCall.Services
{
    internal class HttpDataService:IhttpDataServices
    {

        private static Guid ProgramCaptchaID = Guid.NewGuid();

        const string ApiKey = "ACbaAS324hnaASD324bzZwq41";

        private const string DefaultMainIp = "localhost:5005";

        private readonly IStatusServices _statusServices;

        private readonly IEncryptServices _encryptServices;

        public HttpDataService(IStatusServices statusServices,IEncryptServices encryptServices)
        {
            _statusServices = statusServices;

            _encryptServices = encryptServices;
        }


       private static HttpClientHandler clientHandler = new()
       {
           ServerCertificateCustomValidationCallback = ((sender, cert, chain, sslPolicyErrors) => { return true; })
       };

       private static HttpClient httpClient = new(clientHandler)
       {
           Timeout = TimeSpan.FromSeconds(10),
           DefaultRequestHeaders = { { "ApiKey", ApiKey } },

       };


       public async Task<string> GetAuthorizeToken(Account newAcc, string apiServerIp= DefaultMainIp)
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

            var authModel = new { Login = newAcc.Login, Password = _encryptServices.Encrypt(newAcc.Password),Guid = ProgramCaptchaID };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = new HttpResponseMessage();

            
            try
            {
                response = await httpClient.PostAsync($"https://{apiServerIp}/api/Auth/Authorization", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = ex.Message, isError = true });

                return "invalid";
            }

            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _statusServices.DeleteStatus();

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Authorisation Error", isError = true });

                return "invalid";
            }

        }

        public  async Task<string> Registration(Account newAcc, string capthca, string apiServerIp = DefaultMainIp)
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

            var authModel = new { Login = newAcc.Login, Password = _encryptServices.Encrypt(newAcc.Password), Guid = ProgramCaptchaID,Captcha = capthca };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage response;

            try
            {
                 response = await httpClient.PostAsync($"https://{apiServerIp}/api/auth/Registration", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                _statusServices.ChangeStatus(new StatusMessage { Message = ex.Message, isError = true });

                return null;
            }
          

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _statusServices.DeleteStatus();

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                
                _statusServices.ChangeStatus(new StatusMessage { Message = response.Content.ReadAsStringAsync().Result, isError = true });

                return response.ReasonPhrase;
            }
        }



        public async Task<string> MainServerGetApiIP(string serverName)
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Get API server ip. . ." });

            var json = JsonSerializer.Serialize(serverName);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync($"https://{DefaultMainIp}/api/Server/ServerGetIP", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = ex.Message, isError = true });

                return null;
            }


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                _statusServices.DeleteStatus();

                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect server name", isError = true });

                return null;
            }


        }



        public async Task<Server> ApiServerGetInfo(string apiServerIp)
        {

            _statusServices.ChangeStatus(new StatusMessage { Message = "Get server info. . ." });

            var response = new HttpResponseMessage();

            try
            {
                response = await httpClient.GetAsync($"https://{apiServerIp}/api/Server/ServerGetInfo").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = ex.Message, isError = true });

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
                _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect server name", isError = true });
               
                return null;
            }


        }




        public  async Task<ImagePacket> GetCaptcha(string serverIp = DefaultMainIp)
        {


            _statusServices.ChangeStatus(new StatusMessage { Message = "Get captcha from server. . ."});

            var httpResponseMessage = new HttpResponseMessage();

            var serialize = JsonSerializer.Serialize(ProgramCaptchaID.ToString());

            var stringContent = new StringContent(serialize, Encoding.UTF8, MediaTypeNames.Application.Json);

            try
            {
                 httpResponseMessage = await httpClient.PostAsync($"https://{serverIp}/api/auth/CaptchaGenerator", stringContent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = ex.Message, isError = true });

                return null;
            }

            
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _statusServices.DeleteStatus();

                return JsonSerializer.Deserialize<ImagePacket>(httpResponseMessage.Content.ReadAsStringAsync().Result);
            }
            else
            {
                _statusServices.DeleteStatus();

                return null;
            }
        }


      public  async Task<bool> CheckServerStatus(string serverAdress)
      {

          _statusServices.ChangeStatus(new StatusMessage { Message = "Check server status. . ." });

            string[] serverAddresArray = serverAdress.Split(':');

            if (serverAddresArray.Length == 2)
            {
                try
                {
                    using (var client = new TcpClient(serverAddresArray[0], Convert.ToInt32(serverAddresArray[1]))); 
                        
                    _statusServices.DeleteStatus();

                    return true;
                }
                catch (SocketException ex)
                {
                    _statusServices.ChangeStatus(new StatusMessage { Message = ex.Message,isError = true});

                    return false;
                }
            }
            else
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect Ip adrress", isError = true });
                return false;

            }

           

      }


          public  async Task<string> GetRoleFromJwtToken(string token)
          {
              try
              {
                  dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.Split('.')[1])));
                  return (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
              }
              catch (Exception e)
              {
                  return "User";
              }
          }



    }
}
