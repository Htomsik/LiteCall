using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace LiteCall.Services;

internal class HttpDataService : IHttpDataServices
{
    private const string ApiKey = "ACbaAS324hnaASD324bzZwq41";

    private static readonly Guid ProgramCaptchaId = Guid.NewGuid();


    private static readonly HttpClientHandler ClientHandler = new()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
    };

    private static readonly HttpClient HttpClient = new(ClientHandler)
    {
        Timeout = TimeSpan.FromSeconds(10),
        DefaultRequestHeaders = { { "ApiKey", ApiKey } }
    };

    private readonly IConfiguration _configuration;

    private readonly IEncryptServices _encryptServices;

    private readonly IStatusServices _statusServices;


    public HttpDataService(IStatusServices statusServices, IEncryptServices encryptServices,
        IConfiguration configuration)
    {
        _statusServices = statusServices;

        _encryptServices = encryptServices;

        _configuration = configuration;
    }

    private string? DefaultMainIp => _configuration!.GetSection("MainSever")["MainServerIp"] ?? "localhost:5005";


    public async Task<string> GetAuthorizeToken(RegRecPasswordAccount? newAcc, string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;

        _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

        var authModel = new
            { newAcc.Login, Password = await _encryptServices.Base64Decrypt(newAcc.Password), Guid = ProgramCaptchaId };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;


        try
        {
            response = await HttpClient.PostAsync($"https://{apiServerIp}/api/Auth/Authorization", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage
                { Message = "Failed connect to the server", IsError = true });

            return "invalid";
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return await response.Content.ReadAsStringAsync();
        }

        _statusServices.ChangeStatus(new StatusMessage { Message = "Authorisation Error", IsError = true });

        return "invalid";
    }


    public async Task<string> Registration(RegistrationModel registrationModel, string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;

        _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

        var authModel = new
        {
            registrationModel.RecoveryAccount!.Login,
            Password = await _encryptServices.Base64Decrypt(registrationModel.RecoveryAccount.Password),
            Guid = ProgramCaptchaId,
            registrationModel.Captcha,
            QuestionsId = registrationModel.Question!.Id,
            AnswersecurityQ = registrationModel.QestionAnswer
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await HttpClient.PostAsync($"https://{apiServerIp}/api/Auth/Registration", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

            return null!;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return await response.Content.ReadAsStringAsync();
        }

        _statusServices.ChangeStatus(new StatusMessage
            { Message = response.Content.ReadAsStringAsync().Result, IsError = true });

        return response.ReasonPhrase!;
    }


    public async Task<string?> MainServerGetApiIp(string? serverName)
    {
        _statusServices.ChangeStatus(new StatusMessage { Message = "Get API server ip. . ." });

        var json = JsonSerializer.Serialize(serverName);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await HttpClient.PostAsync($"https://{DefaultMainIp}/api/Server/ServerGetIP", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

            return null;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return response.Content.ReadAsStringAsync().Result;
        }

        _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect server name", IsError = true });

        return null;
    }


    public async Task<Server?> ApiServerGetInfo(string? apiServerIp)
    {
        _statusServices.ChangeStatus(new StatusMessage { Message = "Get server info. . ." });

        HttpResponseMessage response;

        try
        {
            response = await HttpClient.GetAsync($"https://{apiServerIp}/api/Server/ServerGetInfo")
                .ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

            return null;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return JsonSerializer.Deserialize<Server>(response.Content.ReadAsStringAsync().Result);
        }

        _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect server name", IsError = true });

        return null;
    }


    public async Task<ImagePacket?> GetCaptcha(string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;

        _statusServices.ChangeStatus(new StatusMessage { Message = "Get captcha from server. . ." });

        HttpResponseMessage httpResponseMessage;

        var serialize = JsonSerializer.Serialize(ProgramCaptchaId.ToString());

        var stringContent = new StringContent(serialize, Encoding.UTF8, MediaTypeNames.Application.Json);

        try
        {
            httpResponseMessage = await HttpClient
                .PostAsync($"https://{apiServerIp}/api/auth/CaptchaGenerator", stringContent).ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

            return null;
        }


        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return JsonSerializer.Deserialize<ImagePacket>(httpResponseMessage.Content.ReadAsStringAsync().Result);
        }

        _statusServices.DeleteStatus();

        return null;
    }


    public async Task<bool> CheckServerStatus(string? serverAddress)
    {
        _statusServices.ChangeStatus(new StatusMessage { Message = "Check server status. . ." });

        var serverAddressArray = serverAddress!.Split(':');

        if (serverAddressArray.Length == 2)
            try
            {
                using (new TcpClient(serverAddressArray[0], Convert.ToInt32(serverAddressArray[1])))
                {
                }

                _statusServices.DeleteStatus();

                return true;
            }
            catch
            {
                _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

                return false;
            }

        _statusServices.ChangeStatus(new StatusMessage { Message = "Incorrect Ip address", IsError = true });

        return false;
    }

    public async Task<List<Question>?> GetPasswordRecoveryQuestions(string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;


        _statusServices.ChangeStatus(new StatusMessage { Message = "Get Qestions from server. . ." });

        HttpResponseMessage response;

        try
        {
            response = await HttpClient.GetAsync($"https://{apiServerIp}/api/auth/SecurityQuestions")
                .ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

            return null;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return JsonSerializer.Deserialize<List<Question>>(response.Content.ReadAsStringAsync().Result);
        }

        _statusServices.ChangeStatus(new StatusMessage { Message = "Unknown error", IsError = true });

        return new List<Question>();
    }

    public async Task<bool> PasswordRecovery(RecoveryModel recoveryModel, string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;

        _statusServices.ChangeStatus(new StatusMessage { Message = "Connect to server. . ." });

        var authModel = new
        {
            recoveryModel.RecoveryAccount!.Login,
            newPassword = await _encryptServices.Base64Decrypt(recoveryModel.RecoveryAccount.Password),
            QuestionsId = recoveryModel.Question!.Id, AnswersecurityQ = recoveryModel.QestionAnswer
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await HttpClient
                .PostAsync($"https://{apiServerIp}/api/Auth/СhangePasswordbySecurityQuestions", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage { Message = "Server connection error", IsError = true });

            return false;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusServices.DeleteStatus();

            return true;
        }

        _statusServices.ChangeStatus(new StatusMessage { Message = "Unknown error", IsError = true });

        return false;
    }


    public async Task<bool> PostSaveServersUserOnMainServer(Account? currentAccount, AppSavedServers savedServerAccounts)
    {
        var jsonServers = JsonSerializer.Serialize(savedServerAccounts,
            new JsonSerializerOptions
                { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        var saveModel = new
        {
            currentAccount.Login,
            Password = await _encryptServices.Base64Decrypt(currentAccount.Password),
            SaveServers = jsonServers,
            DateSynch = savedServerAccounts.LastUpdated
        };

        var json = JsonSerializer.Serialize(saveModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await HttpClient.PostAsync($"https://{DefaultMainIp}/api/Server/SaveServersUser", content)
                .ConfigureAwait(false);
        }
        catch
        {
            return false;
        }

        return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<AppSavedServers> GetSaveServersUserOnMainServer(Account? currentAccount, DateTime? dateSynch)
    {
        var authModel = new
        {
            currentAccount.Login, Password = await _encryptServices.Base64Decrypt(currentAccount.Password),
            DateSynch = dateSynch
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await HttpClient.PostAsync($"https://{DefaultMainIp}/api/Server/GetServersUser", content)
                .ConfigureAwait(false);
        }
        catch
        {
            return null!;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonNonServerResponse = await response.Content.ReadAsStringAsync();

            var jsonCollection = JsonSerializer.Deserialize<AppSavedServers>(jsonNonServerResponse);

            var newAppSavedServers = new AppSavedServers
            {
                LastUpdated = DateTime.Now,
                ServersAccounts = jsonCollection!.ServersAccounts
            };

            return newAppSavedServers;
        }

        return null!;
    }


    public async Task<string> GetRoleFromJwtToken(string token)
    {
        try
        {
            dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.Split('.')[1])))!;
            return (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        }
        catch
        {
            return "User";
        }
    }
}