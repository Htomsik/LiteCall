using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


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


       public async Task<string> GetAuthorizeToken(Reg_Rec_PasswordAccount newAcc, string apiServerIp = DefaultMainIp)
        {


            if (apiServerIp == null)
            {
                apiServerIp = DefaultMainIp;
            }

            _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

            var authModel = new { Login = newAcc.Login, Password = await _encryptServices.Base64Decrypt(newAcc.Password),Guid = ProgramCaptchaID };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage response;

            
            try
            {
                response = await httpClient.PostAsync($"https://{apiServerIp}/api/Auth/Authorization", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed connect to the server", isError = true });

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



        public  async Task<string> Registration(RegistrationModel registrationModel, string apiServerIp = DefaultMainIp)
        {


            if (apiServerIp == null)
            {
                apiServerIp = DefaultMainIp;
            }

            _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

            var authModel = new { Login = registrationModel.recoveryAccount.Login,
                Password = await _encryptServices.Base64Decrypt(registrationModel.recoveryAccount.Password), 
                Guid = ProgramCaptchaID,
                Captcha = registrationModel.Captcha,
                QuestionsId = registrationModel.Question.Id, 
                AnswersecurityQ = registrationModel.QestionAnswer};

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage response;

            try
            {
                 response = await httpClient.PostAsync($"https://{apiServerIp}/api/Auth/Registration", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed to connect to the server", isError = true });

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


        public async Task<string> MainServerGetApiIp(string serverName)
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


                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed connect to the server", isError = true });

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

            HttpResponseMessage response;

            try
            {
                response = await httpClient.GetAsync($"https://{apiServerIp}/api/Server/ServerGetInfo").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed to connect to the server", isError = true });

                return null;
            }


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _statusServices.DeleteStatus();

                return JsonSerializer.Deserialize<Server>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect server name", isError = true });
               
                return null;
            }


        }




        public  async Task<ImagePacket?> GetCaptcha(string apiServerIp = DefaultMainIp)
        {
            if (apiServerIp == null)
            {
                apiServerIp = DefaultMainIp;
            }

            _statusServices.ChangeStatus(new StatusMessage { Message = "Get captcha from server. . ."});

            HttpResponseMessage httpResponseMessage;

            var serialize = JsonSerializer.Serialize(ProgramCaptchaID.ToString());

            var stringContent = new StringContent(serialize, Encoding.UTF8, MediaTypeNames.Application.Json);

            try
            {
                 httpResponseMessage = await httpClient.PostAsync($"https://{apiServerIp}/api/auth/CaptchaGenerator", stringContent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Failed to connect to the server", isError = true });

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
                   using (var client =  new TcpClient(serverAddresArray[0], Convert.ToInt32(serverAddresArray[1]))); 
                        
                    _statusServices.DeleteStatus();

                    return true;
                }
                catch (SocketException ex)
                {
                    _statusServices.ChangeStatus(new StatusMessage { Message = "Failed to connect to the server", isError = true });

                    return false;
                }
            }
            else
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect Ip adrress", isError = true });

                return false;

            }

      }

      public async Task<List<Question>> GetPasswordRecoveryQestions(string apiServerIp = DefaultMainIp)
      {

          if (apiServerIp == null)
          {
              apiServerIp = DefaultMainIp;
          }


            _statusServices.ChangeStatus(new StatusMessage { Message = "Get Qestions from server. . ." });

          HttpResponseMessage response;

          try
          {
              response = await httpClient.GetAsync($"https://{apiServerIp}/api/auth/SecurityQuestions").ConfigureAwait(false);
          }
          catch (Exception ex)
          {

              _statusServices.ChangeStatus(new StatusMessage { Message = "Failed to connect to the server", isError = true });

              return null;
          }


          if (response.StatusCode == System.Net.HttpStatusCode.OK)
          {
              _statusServices.DeleteStatus();

              return JsonSerializer.Deserialize<List<Question>>(response.Content.ReadAsStringAsync().Result);
          }
          else
          {

              _statusServices.ChangeStatus(new StatusMessage { Message = "Unknown error", isError = true });

              return new List<Question>();
          }


        
      }

        public async Task<bool> PasswordRecovery(RecoveryModel recoveryModel, string apiServerIp = DefaultMainIp)
        {

            if (apiServerIp == null)
            {
                apiServerIp = DefaultMainIp;
            }

            _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

          var authModel = new { Login = recoveryModel.recoveryAccount.Login, newPassword = _encryptServices.Sha1Encrypt(recoveryModel.recoveryAccount.Password), QuestionsId = recoveryModel.Question.Id, AnswersecurityQ = recoveryModel.QestionAnswer};

          var json = JsonSerializer.Serialize(authModel);

          var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

          HttpResponseMessage response;

          try
          {
              response = await httpClient.PostAsync($"https://{apiServerIp}/api/Auth/СhangePasswordbySecurityQuestions", content).ConfigureAwait(false);
          }
          catch (Exception ex)
          {

              _statusServices.ChangeStatus(new StatusMessage { Message = "Failed to connect to the server", isError = true });

              return false;
          }


          if (response.StatusCode == System.Net.HttpStatusCode.OK)
          {
              _statusServices.DeleteStatus();

              return true;
          }
          else
          {

              _statusServices.ChangeStatus(new StatusMessage { Message = "Unknown error", isError = true });

              return false;
          }
        }

        public async Task<bool> PostSaveServersUserOnMainServer(Account currentAccount, AppSavedServers savedServerAccounts)
        {

            if (string.IsNullOrEmpty(currentAccount.Password))
            {
                return false;
            }
            else if (savedServerAccounts?.ServersAccounts?.Count == 0)
            {
                return false;
            }

            var jsonServers = JsonSerializer.Serialize(savedServerAccounts, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true });
            
            var saveModel = new
            {
                Login = currentAccount.Login,
                Password = await _encryptServices.Base64Decrypt(currentAccount.Password),
                SaveServers = jsonServers,
                DateSynch = savedServerAccounts.LastUpdated
            };

            var json = JsonSerializer.Serialize(saveModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync($"https://{DefaultMainIp}/api/Server/SaveServersUser", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                return false;
            }

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public async Task<AppSavedServers> GetSaveServersUserOnMainServer(Account currentAccount, AppSavedServers savedServerAccounts)
        {

            if (string.IsNullOrEmpty(currentAccount.Password))
            {
                return new AppSavedServers();
            }

            var authModel = new { Login = currentAccount.Login, Password = await _encryptServices.Base64Decrypt(currentAccount.Password), DateSynch = savedServerAccounts.LastUpdated };

            var json = JsonSerializer.Serialize(authModel);

            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync($"https://{DefaultMainIp}/api/Server/GetServersUser", content).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                
                return new AppSavedServers();
            }


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                
                var jsonNonServerResponse = await response.Content.ReadAsStringAsync();

                var jsonCollection = JsonSerializer.Deserialize<AppSavedServers>(jsonNonServerResponse);

                AppSavedServers newAppSavedServers = new AppSavedServers
                {
                    LastUpdated = DateTime.Now,
                    ServersAccounts = jsonCollection.ServersAccounts
                };

                return newAppSavedServers;
            }
            else
            {

                return new AppSavedServers();
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
