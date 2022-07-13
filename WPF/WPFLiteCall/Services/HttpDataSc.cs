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
using Core.Models;
using Core.Models.AccountManagement;
using Core.Models.AppInfrastructure;
using Core.Models.AppInfrastructure.StateStatuses;
using Core.Models.Images;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using Core.Services;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Services.Interfaces.Extra;
using Core.Stores.Connections;
using LiteCall.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace LiteCall.Services;

internal sealed class HttpDataSc : IHttpDataSc
{
    private static readonly Guid ProgramCaptchaId = Guid.NewGuid();


    private readonly IConfiguration _configuration;

    private readonly IEncryptSc _encryptSc;

    private readonly HttpClientStore _httpClientStore;

    private readonly IStatusSc _statusSc;


    public HttpDataSc(IStatusSc statusSc, IEncryptSc encryptSc,
        IConfiguration configuration, HttpClientStore httpClientStore)
    {
        _statusSc = statusSc;

        _encryptSc = encryptSc;

        _configuration = configuration;

        _httpClientStore = httpClientStore;
    }

    private string? DefaultMainIp => _configuration!.GetSection("MainSever")["MainServerIp"] ?? "localhost:5005";


    public async Task<string> GetAuthorizeToken(RegistrationUser? newAcc, string? apiServerIp = null)
    {
        apiServerIp ??= DefaultMainIp;


        _statusSc.ChangeStatus(ExecutionActionStates.ServerConnection);

        var authModel = new
        {
            newAcc!.Login, Password = await _encryptSc.Base64Decrypt(newAcc.Password), Guid = ProgramCaptchaId
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;


        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{apiServerIp}/api/Auth/Authorization", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return "invalid";
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            return await response.Content.ReadAsStringAsync();
        }


        _statusSc.ChangeStatus(ExecutionErrorStates.AuthorizationFailed);

        return "invalid";
    }

    
    public async Task<string> Registration(RegistrationModel? registrationModel, string? apiServerIp = null)
    {
        apiServerIp ??= DefaultMainIp;


        _statusSc.ChangeStatus(ExecutionActionStates.ServerConnection);

        var authModel = new
        {
            registrationModel!.RecoveryAccount!.Login,
            Password = await _encryptSc.Base64Decrypt(registrationModel.RecoveryAccount.Password),
            Guid = ProgramCaptchaId,
            registrationModel.Captcha,
            QuestionsId = registrationModel.Question!.Id,
            AnswersecurityQ = registrationModel.QuestionAnswer
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{apiServerIp}/api/Auth/Registration", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return null!;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            return await response.Content.ReadAsStringAsync();
        }


        _statusSc.ChangeStatus(ExecutionErrorStates.RegistrationFailed);
        return response.ReasonPhrase!;
    }

    public async Task<string?> MainServerGetApiIp(string? serverName)
    {
        _statusSc.ChangeStatus(ExecutionActionStates.GettingInfoAboutServer);

        var json = JsonSerializer.Serialize(serverName);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{DefaultMainIp}/api/Server/ServerGetIP", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return null;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            return response.Content.ReadAsStringAsync().Result;
        }


        _statusSc.ChangeStatus(ExecutionErrorStates.IncorrectServerNameOrIp);

        return null;
    }

    public async Task<Server?> ApiServerGetInfo(string? apiServerIp)
    {
        _statusSc.ChangeStatus(ExecutionActionStates.GettingInfoAboutServer);

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .GetAsync($"https://{apiServerIp}/api/Server/ServerGetInfo")
                .ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return null;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            return JsonSerializer.Deserialize<Server>(response.Content.ReadAsStringAsync().Result);
        }


        _statusSc.ChangeStatus(ExecutionErrorStates.IncorrectServerNameOrIp);

        return null;
    }

    public async Task<ImagePacket?> GetCaptcha(string? apiServerIp = null)
    {
        apiServerIp ??= DefaultMainIp;


        _statusSc.ChangeStatus(ExecutionActionStates.GettingCaptcha);

        HttpResponseMessage httpResponseMessage;

        var serialize = JsonSerializer.Serialize(ProgramCaptchaId.ToString());

        var stringContent = new StringContent(serialize, Encoding.UTF8, MediaTypeNames.Application.Json);

        try
        {
            httpResponseMessage = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{apiServerIp}/api/auth/CaptchaGenerator", stringContent).ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return null;
        }


        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            var test = JsonSerializer.Deserialize<ImagePacket>(await httpResponseMessage.Content.ReadAsStringAsync());
            
            return test;
        }

        _statusSc.DeleteStatus();

        return null;
    }

    public Task<bool> CheckServerStatus(string? serverAddress)
    {
        _statusSc.ChangeStatus(ExecutionActionStates.ServerConnection);

        var serverAddressArray = serverAddress!.Split(':');

        if (serverAddressArray.Length == 2)
            try
            {
                using (new TcpClient(serverAddressArray[0], Convert.ToInt32(serverAddressArray[1])))
                {
                }

                _statusSc.DeleteStatus();

                return Task.FromResult(true);
            }
            catch
            {
                _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

                return Task.FromResult(false);
            }

        _statusSc.ChangeStatus(ExecutionErrorStates.IncorrectServerIp);

        return Task.FromResult(false);
    }

    public async Task<List<Question>?> GetPasswordRecoveryQuestions(string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;


        _statusSc.ChangeStatus(new AppExecutionState { Message = "Get Questions from server. . ." });

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .GetAsync($"https://{apiServerIp}/api/auth/SecurityQuestions")
                .ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return null;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            return JsonSerializer.Deserialize<List<Question>>(response.Content.ReadAsStringAsync().Result);
        }

        _statusSc.ChangeStatus(ExecutionErrorStates.UnknownError);

        _statusSc.ChangeStatus(ExecutionErrorStates.UnknownError);

        return new List<Question>();
    }

    public async Task<bool> PasswordRecovery(RecoveryModel recoveryModel, string? apiServerIp = null)
    {
        if (apiServerIp == null) apiServerIp = DefaultMainIp;

        _statusSc.ChangeStatus(ExecutionActionStates.ServerConnection);

        var authModel = new
        {
            recoveryModel.RecoveryAccount!.Login,
            newPassword = await _encryptSc.Base64Decrypt(recoveryModel.RecoveryAccount.Password),
            QuestionsId = recoveryModel.Question!.Id, AnswersecurityQ = recoveryModel.QuestionAnswer
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{apiServerIp}/api/Auth/СhangePasswordbySecurityQuestions", content)
                .ConfigureAwait(false);
        }
        catch
        {
            _statusSc.ChangeStatus(ExecutionErrorStates.ServerConnectionFailed);

            return false;
        }


        if (response.StatusCode == HttpStatusCode.OK)
        {
            _statusSc.DeleteStatus();

            return true;
        }

        _statusSc.ChangeStatus(ExecutionErrorStates.UnknownError);

        return false;
    }

    public async Task<bool> PostSaveServersUserOnMainServer(Account? currentAccount,
        AppSavedServers savedServerAccounts)
    {
        var jsonServers = JsonSerializer.Serialize(savedServerAccounts,
            new JsonSerializerOptions
                { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        var saveModel = new
        {
            currentAccount!.Login,
            Password = await _encryptSc.Base64Decrypt(currentAccount.Password),
            SaveServers = jsonServers,
            DateSynch = savedServerAccounts.LastUpdated
        };

        var json = JsonSerializer.Serialize(saveModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{DefaultMainIp}/api/Server/SaveServersUser", content)
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
            currentAccount!.Login, Password = await _encryptSc.Base64Decrypt(currentAccount.Password),
            DateSynch = dateSynch
        };

        var json = JsonSerializer.Serialize(authModel);

        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpResponseMessage response;

        try
        {
            response = await _httpClientStore.CurrentHttpClient
                .PostAsync($"https://{DefaultMainIp}/api/Server/GetServersUser", content)
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

    public Task<string> GetRoleFromJwtToken(string token)
    {
        try
        {
            dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token.Split('.')[1])))!;
            return Task.FromResult((string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
        }
        catch
        {
            return Task.FromResult("User");
        }
    }
}